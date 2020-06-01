using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aptacode.CSharp.Core.Http;
using Aptacode.CSharp.Utilities.Persistence;
using Newtonsoft.Json;

namespace Aptacode.CSharp.Core.Services
{
    public abstract class GenericHttpApiServiceClient<TEntity> : IGenericHttpService<TEntity> where TEntity : IEntity
    {
        protected static HttpClient HttpClient = new HttpClient();
        protected readonly HttpRouteBuilder ApiRouteBuilder;
        protected IAccessTokenService AuthService;

        protected GenericHttpApiServiceClient(IAccessTokenService authService, ServerAddress serverAddress,
            string apiRoute,
            string controllerRoute)
        {
            ApiRouteBuilder = new HttpRouteBuilder(serverAddress, apiRoute, controllerRoute);
            AuthService = authService;
        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            var response = await HttpClient.SendAsync(GetRequestTemplate(HttpMethod.Get, ApiRouteBuilder.GetRoute()));
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<TEntity>>(body);
        }

        public async Task<IEnumerable<TEntity>> Get(int id)
        {
            var response =
                await HttpClient.SendAsync(GetRequestTemplate(HttpMethod.Get, ApiRouteBuilder.GetRoute(id.ToString())));
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<TEntity>>(body);
        }

        public async Task<TEntity> Push(TEntity entity)
        {
            var req = GetRequestTemplate(HttpMethod.Post, ApiRouteBuilder.GetRoute());
            req.Content = new StringContent(JsonConvert.SerializeObject(entity));
            var response = await HttpClient.SendAsync(req);
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TEntity>(body);
        }

        public async Task Put(TEntity entity)
        {
            var req = GetRequestTemplate(HttpMethod.Put, ApiRouteBuilder.GetRoute(entity.Id.ToString()));
            req.Content = new StringContent(JsonConvert.SerializeObject(entity));
            var response = await HttpClient.SendAsync(req);
        }

        public async Task Delete(int id)
        {
            var req = GetRequestTemplate(HttpMethod.Delete, ApiRouteBuilder.GetRoute(id.ToString()));
            await HttpClient.SendAsync(req);
        }

        protected HttpRequestMessage GetRequestTemplate(HttpMethod method, string endpoint)
        {
            var accessToken = AuthService.GetAccessToken();
            if (accessToken == null)
            {
                throw new Exception("You are not authorized to view this content");
            }

            var req = new HttpRequestMessage();
            req.Method = method;
            req.RequestUri = new Uri(endpoint);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return req;
        }
    }
}