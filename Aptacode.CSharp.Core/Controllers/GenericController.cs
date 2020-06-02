using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.Repository;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Controllers
{
    public abstract class GenericController<TEntity> : ControllerBase where TEntity : IEntity
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IGenericUnitOfWork UnitOfWork;

        protected GenericController(IGenericUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Repository = unitOfWork.Repository<TEntity>();
        }

        protected virtual async Task<(bool, StatusCodeResult)> IsPutAuthorized(TEntity entity)
        {
            return (true, Ok());
        }

        protected virtual async Task<(bool, StatusCodeResult)> IsPostAuthorized(TEntity entity)
        {
            return (true, Ok());
        }

        protected virtual async Task<(bool, StatusCodeResult)> IsGetAuthorized()
        {
            return (true, Ok());
        }

        protected virtual async Task<(bool, StatusCodeResult)> IsGetAuthorized(int id)
        {
            return (true, Ok());
        }

        protected virtual async Task<(bool, StatusCodeResult)> IsDeleteAuthorized(TEntity entity)
        {
            return (true, Ok());
        }

        protected virtual async Task<IActionResult> Put(int id, TEntity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            var authorizedResult = await IsPutAuthorized(entity).ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            await Repository.Update(entity).ConfigureAwait(false);

            try
            {
                await UnitOfWork.Commit().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException) when (!EntityExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        protected async Task<ActionResult<TEntity>> Post(TEntity entity)
        {
            var authorizedResult = await IsPostAuthorized(entity).ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            await Repository.Create(entity).ConfigureAwait(false);
            await UnitOfWork.Commit().ConfigureAwait(false);

            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        protected virtual async Task<ActionResult<IEnumerable<TEntity>>> Get()
        {
            var authorizedResult = await IsGetAuthorized().ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            var results = await Repository.AsQueryable().ToListAsync().ConfigureAwait(false);
            return Ok(results);
        }

        protected virtual async Task<ActionResult<TEntity>> Get(int id)
        {
            var authorizedResult = await IsGetAuthorized(id).ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            var result = await Repository.Get(id).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        protected virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await Repository.Get(id).ConfigureAwait(false);
            if (entity == null)
            {
                return NotFound();
            }

            var authorizedResult = await IsDeleteAuthorized(entity).ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            await Repository.Delete(id).ConfigureAwait(false);
            await UnitOfWork.Commit().ConfigureAwait(false);

            return Ok();
        }

        private bool EntityExists(int id)
        {
            return Repository.AsQueryable().Any(e => e.Id == id);
        }
    }
}