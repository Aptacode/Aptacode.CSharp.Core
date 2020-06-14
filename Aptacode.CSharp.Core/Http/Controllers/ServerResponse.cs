using System.Net;
using Aptacode.CSharp.Common.Http.Services.Responses;

namespace Aptacode.CSharp.Core.Http.Controllers
{
    public class ServerResponse<T> : ServiceResponse<T>
    {
        public ServerResponse(HttpStatusCode statusCode, string message, T value) : base(value, message)
        {
            StatusCode = statusCode;
        }

        public ServerResponse(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        #region Factory Methods

        public static ServerResponse<T> Create(HttpStatusCode statusCode, string message, T content) =>
            new ServerResponse<T>(statusCode, message, content);

        public static ServerResponse<T> Create(HttpStatusCode statusCode, string message) =>
            new ServerResponse<T>(statusCode, message);

        #endregion
    }
}