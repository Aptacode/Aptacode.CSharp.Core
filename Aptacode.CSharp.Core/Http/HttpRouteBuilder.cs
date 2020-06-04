namespace Aptacode.CSharp.Core.Http
{
    public class HttpRouteBuilder
    {
        private const string RouteSeparator = "/";
        public string ApiBaseRoute { get; }
        public ServerAddress ServerAddress { get; }

        public HttpRouteBuilder(ServerAddress serverAddress, params string[] baseRouteSegments)
        {
            ServerAddress = serverAddress;
            ApiBaseRoute = $"{serverAddress}{string.Join(RouteSeparator, baseRouteSegments)}";
        }

        public string BuildRoute(params string[] routeSegments)
        {
            return $@"{ApiBaseRoute}{string.Join(RouteSeparator, routeSegments)}";
        }
    }
}