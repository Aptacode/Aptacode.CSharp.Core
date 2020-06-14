using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aptacode.CSharp.Common.Persistence;
using Aptacode.CSharp.Common.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Http.Controllers
{
    public abstract class GenericController : ControllerBase
    {
        protected readonly IGenericUnitOfWork UnitOfWork;

        protected GenericController(IGenericUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new NullReferenceException("IGenericUnitOfWork was null");
        }

        protected virtual async Task<ServerResponse<T>> Post<T>(int id, T entity, Validator<T> validator = null)
            where T : IEntity
        {
            if (entity == null)
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "Null Entity was given");
            }

            if (id != entity.Id)
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "Entity's Id did not match");
            }

            if (validator != null)
            {
                var result = await validator(entity).ConfigureAwait(false);
                if (!result.HasValue || !result.Value)
                {
                    return new ServerResponse<T>(result.StatusCode, result.Message);
                }
            }

            try
            {
                await UnitOfWork.Repository<T>().Update(entity).ConfigureAwait(false);
                await UnitOfWork.Commit().ConfigureAwait(false);
                return new ServerResponse<T>(HttpStatusCode.OK, "Success", entity);
            }
            catch
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        protected virtual async Task<ServerResponse<T>> Put<T>(T entity, Validator<T> validator = null)
            where T : IEntity
        {
            if (entity == null)
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "Null Entity was given");
            }

            if (validator != null)
            {
                var result = await validator(entity).ConfigureAwait(false);
                if (!result.HasValue || !result.Value)
                {
                    return new ServerResponse<T>(result.StatusCode, result.Message);
                }
            }

            try
            {
                await UnitOfWork.Repository<T>().Create(entity).ConfigureAwait(false);
                await UnitOfWork.Commit().ConfigureAwait(false);
                return new ServerResponse<T>(HttpStatusCode.OK, "Success", entity);
            }
            catch
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        protected virtual async Task<ServerResponse<IEnumerable<T>>> Get<T>(
            Query<T> queryExpression, Validator validator = null)
            where T : IEntity
        {
            if (validator != null)
            {
                var result = await validator().ConfigureAwait(false);
                if (!result.HasValue || !result.Value)
                {
                    return new ServerResponse<IEnumerable<T>>(result.StatusCode, result.Message);
                }
            }

            try
            {
                var results = await UnitOfWork.Repository<T>().AsQueryable().Where(queryExpression()).ToListAsync()
                    .ConfigureAwait(false);
                return new ServerResponse<IEnumerable<T>>(HttpStatusCode.OK, "Success", results);
            }
            catch
            {
                return new ServerResponse<IEnumerable<T>>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        protected virtual async Task<ServerResponse<IEnumerable<T>>> Get<T>(
            Validator validator = null) where T : IEntity
        {
            if (validator != null)
            {
                var result = await validator().ConfigureAwait(false);
                if (!result.HasValue || !result.Value)
                {
                    return new ServerResponse<IEnumerable<T>>(result.StatusCode, result.Message);
                }
            }

            try
            {
                var results = await UnitOfWork.Repository<T>().AsQueryable().ToListAsync().ConfigureAwait(false);
                return new ServerResponse<IEnumerable<T>>(HttpStatusCode.OK, "Success", results);
            }
            catch
            {
                return new ServerResponse<IEnumerable<T>>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        protected virtual async Task<ServerResponse<T>> Get<T>(int id,
            Validator<int> validator = null) where T : IEntity
        {
            if (validator != null)
            {
                var result = await validator(id).ConfigureAwait(false);
                if (!result.HasValue || !result.Value)
                {
                    return new ServerResponse<T>(result.StatusCode, result.Message);
                }
            }

            try
            {
                var result = await UnitOfWork.Repository<T>().Get(id).ConfigureAwait(false);
                if (result == null)
                {
                    return new ServerResponse<T>(HttpStatusCode.BadRequest, "Not Found");
                }

                return new ServerResponse<T>(HttpStatusCode.OK, "Success", result);
            }
            catch
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        protected virtual async Task<ServerResponse<bool>> Delete<TEntity>(int id,
            Validator<int> validator = null) where TEntity : IEntity
        {
            if (validator != null)
            {
                var result = await validator(id).ConfigureAwait(false);
                if (!result.HasValue || !result.Value)
                {
                    return new ServerResponse<bool>(result.StatusCode, result.Message);
                }
            }

            try
            {
                await UnitOfWork.Repository<TEntity>().Delete(id).ConfigureAwait(false);
                await UnitOfWork.Commit().ConfigureAwait(false);

                return new ServerResponse<bool>(HttpStatusCode.OK, "Success", true);
            }
            catch
            {
                return new ServerResponse<bool>(HttpStatusCode.BadRequest, "DataBase Error", false);
            }
        }


        protected ActionResult<T> ToActionResult<T>(ServerResponse<T> response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(response.Value);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response.Message);
                case HttpStatusCode.NotFound:
                    return NotFound(response.Message);
                default:
                    return BadRequest(response.Message);
            }
        }
    }
}