using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aptacode.CSharp.Core.Http;
using Aptacode.CSharp.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Aptacode.CSharp.Core.Services
{
    public abstract class GenericHttpApiServiceClient
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

        protected async Task<IEnumerable<TViewModel>> Get<TViewModel>(params string[] routeSegments)
        {
            var response = await HttpClient.SendAsync(GetRequestTemplate(HttpMethod.Get, ApiRouteBuilder.GetRoute(routeSegments)));
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<TViewModel>>(body);
        }

        protected async Task<TViewModel> Get<TViewModel>(int id, params string[] routeSegments)
        {
            var route = new List<string>(routeSegments){ id.ToString() };

            var response = await HttpClient.SendAsync(GetRequestTemplate(HttpMethod.Get, ApiRouteBuilder.GetRoute(route.ToArray())));

            if (!response.IsSuccessStatusCode) return default;
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TViewModel>(body);
        }

        protected async Task<TGetViewModel> Put<TGetViewModel, TPutViewModel>(TPutViewModel entity, params string[] routeSegments)
        {
            var req = GetRequestTemplate(HttpMethod.Put, ApiRouteBuilder.GetRoute(routeSegments));
            req.Content = new StringContent(JsonConvert.SerializeObject(entity));
            var response = await HttpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode) return default;

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TGetViewModel>(body);

        }

        protected async Task<TGetViewModel> Post<TGetViewModel, TPutViewModel>(int id, TPutViewModel entity, params string[] routeSegments)
        {
            var route = new List<string>(routeSegments) { id.ToString() };

            var req = GetRequestTemplate(HttpMethod.Put, ApiRouteBuilder.GetRoute(route.ToArray()));
            req.Content = new StringContent(JsonConvert.SerializeObject(entity));
            var response = await HttpClient.SendAsync(req);
            if (!response.IsSuccessStatusCode) return default;

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TGetViewModel>(body);
        }

        protected async Task<bool> Delete(int id, params string[] routeSegments)
        {
            var route = new List<string>(routeSegments) { id.ToString() };

            var req = GetRequestTemplate(HttpMethod.Delete, ApiRouteBuilder.GetRoute(route.ToArray()));
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