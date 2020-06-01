using System;
using System.Linq;
using System.Security.Claims;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public abstract class BearerGenericController<TEntity> : GenericController<TEntity> where TEntity : IEntity
    {
        protected BearerGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected int GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, out var result))
            {
                return result;
            }

            throw new ArgumentException("Invalid user token");
        }

        protected string GetUserRole()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
            {
                throw new ArgumentException("Invalid user token");
            }

            return userRole;
        }
    }
}