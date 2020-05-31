using Aptacode.CSharp.NetCore.Services;

namespace Aptacode.CSharp.NetCore.Http
{
    public class HttpRouteBuilder
    {
        private readonly string _apiBaseRoute;
        private const string RouteSeparator = "/";

        public HttpRouteBuilder(ServerAddress serverAddress, string apiBaseRoute, string controllerRoute)
        {
            _apiBaseRoute = $"{serverAddress}/{apiBaseRoute}/{controllerRoute}/";
        }

        public string GetRoute(params string[] routeSegments) => $"{_apiBaseRoute}{string.Join(RouteSeparator, routeSegments)}";
    }
}
