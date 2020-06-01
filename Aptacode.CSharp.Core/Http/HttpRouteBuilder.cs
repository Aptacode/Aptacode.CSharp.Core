namespace Aptacode.CSharp.Core.Http
{
    public class HttpRouteBuilder
    {
        private const string RouteSeparator = "/";
        private readonly string _apiBaseRoute;

        public HttpRouteBuilder(ServerAddress serverAddress, string apiBaseRoute, string controllerRoute)
        {
            _apiBaseRoute = $"{serverAddress}/{apiBaseRoute}/{controllerRoute}/";
        }

        public string GetRoute(params string[] routeSegments)
        {
            return $"{_apiBaseRoute}{string.Join(RouteSeparator, routeSegments)}";
        }
    }
}