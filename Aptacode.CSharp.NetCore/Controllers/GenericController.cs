using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.Repository;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.NetCore.Controllers
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

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, TEntity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
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

        [HttpPost]
        public async Task<ActionResult<TEntity>> PostMessage(TEntity entity)
        {
            await Repository.Create(entity).ConfigureAwait(false);
            await UnitOfWork.Commit().ConfigureAwait(false);

            return CreatedAtAction("GetMessage", new { id = entity.Id }, entity);
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TEntity>>> ReadAll()
        {
            var results = await Repository.AsQueryable().ToListAsync().ConfigureAwait(false);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntity>> Read(int id)
        {
            var result = await Repository.Get(id).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await Repository.Get(id).ConfigureAwait(false);
            if (entity == null)
            {
                return NotFound();
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
