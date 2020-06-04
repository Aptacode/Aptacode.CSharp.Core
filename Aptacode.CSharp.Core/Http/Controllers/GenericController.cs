using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        protected virtual async Task<ActionResult<TEntity>> Post<TEntity>(int id, TEntity entity,
            Func<TEntity, Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            if (id != entity.Id) return BadRequest("Entity Id did not match");

            if (validator != null)
            {
                var (isValid, statusCodeResult) = await validator(entity).ConfigureAwait(false);
                if (!isValid) return statusCodeResult;
            }

            try
            {
                await UnitOfWork.Repository<TEntity>().Update(entity).ConfigureAwait(false);
                await UnitOfWork.Commit().ConfigureAwait(false);
            }
            catch
            {
                return BadRequest();
            }

            return Ok(entity);
        }

        protected virtual async Task<ActionResult<TEntity>> Put<TEntity>(TEntity entity,
            Func<TEntity, Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            if (validator != null)
            {
                var (isValid, statusCodeResult) = await validator(entity).ConfigureAwait(false);
                if (!isValid) return statusCodeResult;
            }

            try
            {
                await UnitOfWork.Repository<TEntity>().Create(entity).ConfigureAwait(false);
                await UnitOfWork.Commit().ConfigureAwait(false);
            }
            catch
            {
                return BadRequest();
            }

            return Ok(entity);
        }

        protected virtual async Task<ActionResult<IEnumerable<TEntity>>> Get<TEntity>(
            Expression<Func<TEntity, bool>> queryExpression, Func<Task<(bool, StatusCodeResult)>> validator = null)
            where TEntity : IEntity
        {
            if (validator != null)
            {
                var (isValid, statusCodeResult) = await validator().ConfigureAwait(false);
                if (!isValid) return statusCodeResult;
            }

            try
            {
                var results = await UnitOfWork.Repository<TEntity>().AsQueryable().Where(queryExpression).ToListAsync()
                    .ConfigureAwait(false);
                return Ok(results);
            }
            catch
            {
                return BadRequest();
            }
        }

        protected virtual async Task<ActionResult<IEnumerable<TEntity>>> Get<TEntity>(
            Func<Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            if (validator != null)
            {
                var (isValid, statusCodeResult) = await validator().ConfigureAwait(false);
                if (!isValid) return statusCodeResult;
            }

            try
            {
                var results = await UnitOfWork.Repository<TEntity>().AsQueryable().ToListAsync().ConfigureAwait(false);
                return Ok(results);
            }
            catch
            {
                return BadRequest();
            }
        }

        protected virtual async Task<ActionResult<TEntity>> Get<TEntity>(int id,
            Func<int, Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            if (validator != null)
            {
                var (isValid, statusCodeResult) = await validator(id).ConfigureAwait(false);
                if (!isValid) return statusCodeResult;
            }

            try
            {
                var result = await UnitOfWork.Repository<TEntity>().Get(id).ConfigureAwait(false);
                if (result == null) return NotFound();

                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        protected virtual async Task<IActionResult> Delete<TEntity>(int id,
            Func<int, Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            if (validator != null)
            {
                var (isValid, statusCodeResult) = await validator(id).ConfigureAwait(false);
                if (!isValid) return statusCodeResult;
            }

            try
            {
                await UnitOfWork.Repository<TEntity>().Delete(id).ConfigureAwait(false);
                await UnitOfWork.Commit().ConfigureAwait(false);
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}