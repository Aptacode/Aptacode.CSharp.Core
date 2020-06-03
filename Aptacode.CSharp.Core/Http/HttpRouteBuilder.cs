namespace Aptacode.CSharp.Core.Http
{
    public class HttpRouteBuilder
    {
        private const string RouteSeparator = "/";
        private readonly string _apiBaseRoute;

        public HttpRouteBuilder(ServerAddress serverAddress, params string[] baseRouteSegments)
        {
            _apiBaseRoute = $"{serverAddress}{string.Join(RouteSeparator, baseRouteSegments)}";
        }

        public string GetRoute(params string[] routeSegments)
        {
            return $"{_apiBaseRoute}{string.Join(RouteSeparator, routeSegments)}";
        }
    }
}