using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.Repository;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aptacode.NetCore
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

        [HttpPut]
        public virtual async Task<IActionResult> Create()
        {
            var bodyContent = await new StreamReader(Request.Body).ReadToEndAsync().ConfigureAwait(false);

            var newEntity = JsonConvert.DeserializeObject<TEntity>(bodyContent);

            await Repository.Create(newEntity).ConfigureAwait(false);
            await UnitOfWork.Commit().ConfigureAwait(false);

            return Ok(newEntity.Id);
        }

        [HttpGet]
        public virtual async Task<IActionResult> ReadAll()
        {
            return Ok(Repository.AsQueryable().Select(e => e.Id));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Read(int id)
        {
            var entity = await Repository.Get(id).ConfigureAwait(false);
            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Update()
        {
            var bodyContent = await new StreamReader(Request.Body).ReadToEndAsync().ConfigureAwait(false);

            var newEntity = JsonConvert.DeserializeObject<TEntity>(bodyContent);

            await Repository.Update(newEntity).ConfigureAwait(false);
            await UnitOfWork.Commit().ConfigureAwait(false);

            return Ok();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            await Repository.Delete(id).ConfigureAwait(false);
            await UnitOfWork.Commit().ConfigureAwait(false);

            return Ok();
        }
    }
}
