using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aptacode.CSharp.Core.Http.Services.Interfaces;
using Newtonsoft.Json;

namespace Aptacode.CSharp.Core.Http.Services
{
    public abstract class GenericHttpApiServiceClient
    {
        protected static HttpClient HttpClient = new HttpClient();
        protected readonly HttpRouteBuilder ApiRouteBuilder;
        protected IAccessTokenService AuthService;

        protected GenericHttpApiServiceClient(IAccessTokenService authService, ServerAddress serverAddress)
        {
            ApiRouteBuilder = new HttpRouteBuilder(serverAddress);
            AuthService = authService;
        }

        protected async Task<IEnumerable<TViewModel>> GetAll<TViewModel>(params string[] routeSegments)
        {
            var response =
                await HttpClient.SendAsync(GetRequestTemplate(HttpMethod.Get, ApiRouteBuilder.GetRoute(routeSegments)));
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<TViewModel>>(body);
        }

        protected async Task<TViewModel> Get<TViewModel>(params string[] routeSegments)
        {
            var response =
                await HttpClient.SendAsync(GetRequestTemplate(HttpMethod.Get, ApiRouteBuilder.GetRoute(routeSegments)));

            if (!response.IsSuccessStatusCode) return default;
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TViewModel>(body);
        }

        protected async Task<TGetViewModel> Put<TGetViewModel, TPutViewModel>(TPutViewModel entity,
            params string[] routeSegments)
        {
            var req = GetRequestTemplate(HttpMethod.Put, ApiRouteBuilder.GetRoute(routeSegments));
            req.Content = new StringContent(JsonConvert.SerializeObject(entity));
            var response = await HttpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode) return default;

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TGetViewModel>(body);
        }

        protected async Task<TGetViewModel> Post<TGetViewModel, TPutViewModel>(TPutViewModel entity,
            params string[] routeSegments)
        {
            var req = GetRequestTemplate(HttpMethod.Put, ApiRouteBuilder.GetRoute(routeSegments));
            req.Content = new StringContent(JsonConvert.SerializeObject(entity));
            var response = await HttpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode) return default;

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TGetViewModel>(body);
        }

        protected async Task<bool> Delete(params string[] routeSegments)
        {
            var req = GetRequestTemplate(HttpMethod.Delete, ApiRouteBuilder.GetRoute(routeSegments));
            var response = await HttpClient.SendAsync(req);
            return response.IsSuccessStatusCode;
        }

        protected HttpRequestMessage GetRequestTemplate(HttpMethod method, string endpoint)
        {
            var accessToken = AuthService.GetAccessToken();
            if (accessToken == null) throw new Exception("You are not authorized to view this content");

            var req = new HttpRequestMessage {Method = method, RequestUri = new Uri(endpoint)};
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return req;
        }
    }
}