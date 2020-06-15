using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Aptacode.CSharp.Common.Persistence;
using Aptacode.CSharp.Common.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Http.Controllers
{
    /// <summary>
    /// Provides a collection of generic Http methods for querying &
    /// returning entities from an IRepository contained within the given IGenericUnitOfWork
    /// </summary>
    public abstract class GenericController : ControllerBase
    {
        protected readonly IGenericUnitOfWork UnitOfWork;

        protected GenericController(IGenericUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new NullReferenceException("IGenericUnitOfWork was null");
        }

        /// <summary>
        /// Finds and updates the given entity in the matching IRepository<T> from the IGenericUnitOfWork
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Inserts a new entity in the matching IRepository<T> from the IGenericUnitOfWork
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a collection of entities found to match the queryExpression in the matching IRepository<T> from the IGenericUnitOfWork
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryExpression"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        protected virtual async Task<ServerResponse<IEnumerable<T>>> Get<T>(Expression<Func<T, bool>> queryExpression = null, Validator validator = null)
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
                IEnumerable<T> results;

                if (queryExpression == null)
                {
                    results = await UnitOfWork.Repository<T>().AsQueryable().ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    results = await UnitOfWork.Repository<T>().AsQueryable().Where(queryExpression).ToListAsync().ConfigureAwait(false);
                }

                return new ServerResponse<IEnumerable<T>>(HttpStatusCode.OK, "Success", results);
            }
            catch
            {
                return new ServerResponse<IEnumerable<T>>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        /// <summary>
        /// Returns the requested entity from the matching IRepository<T> within the IGenericUnitOfWork
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
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
                return result != null ?
                    new ServerResponse<T>(HttpStatusCode.OK, "Success", result) : 
                    new ServerResponse<T>(HttpStatusCode.BadRequest, "Not Found");
            }
            catch
            {
                return new ServerResponse<T>(HttpStatusCode.BadRequest, "DataBase Error");
            }
        }

        /// <summary>
        /// Deletes the requested entity from the matching IRepository<T> within the IGenericUnitOfWork
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts the given ServerResponse<T> into an ActionResult<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
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