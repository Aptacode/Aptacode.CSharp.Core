using System;
using System.Linq;
using System.Security.Claims;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.NetCore.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public abstract class BearerGenericController<TEntity> : GenericController<TEntity> where TEntity : IEntity
    {
        protected BearerGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, out var result))
            {
                return result;
            }

            throw new ArgumentException("Invalid user token");
        }

        public TUserRoles GetUserRole<TUserRoles>() where TUserRoles : struct, Enum
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<TUserRoles>(userRole, out var result))
            {
                return result;
            }

            throw new ArgumentException("Invalid user token");
        }
    }
}