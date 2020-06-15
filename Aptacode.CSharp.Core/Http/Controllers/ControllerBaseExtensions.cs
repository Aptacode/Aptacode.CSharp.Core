using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Aptacode.CSharp.Core.Http.Controllers
{
    /// <summary>
    /// A collection of extension methods for a ControllerBase
    /// </summary>
    public static class ControllerBaseExtensions
    {
        public static int? GetUserId(this ControllerBase controller)
        {
            var userId = controller.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, out var result))
            {
                return result;
            }

            return null;
        }

        public static string GetUserRole(this ControllerBase controller)
        {
            return controller.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
    }
}