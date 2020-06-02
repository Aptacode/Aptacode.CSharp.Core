using System;
using System.Linq;
using System.Security.Claims;

namespace Aptacode.CSharp.Core.Controllers
{
    public static class ControllerUtilities
    {
        public static int GetId(ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, out var result))
            {
                return result;
            }

            throw new ArgumentException("Invalid user token");
        }

        public static string GetRole(ClaimsPrincipal user)
        {
            var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                throw new ArgumentException("Invalid user token");
            }

            return userRole;
        }
    }
}
