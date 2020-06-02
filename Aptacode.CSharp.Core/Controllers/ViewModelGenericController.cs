using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Controllers
{
    public abstract class ViewModelGenericController<TPutViewModel, TGetViewModel, TEntity> : GenericController<TEntity>
        where TPutViewModel : PutViewModel<TEntity>
        where TGetViewModel : GetViewModel<TEntity>
        where TEntity : IEntity
    {
        protected ViewModelGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public abstract TGetViewModel ToViewModel(TEntity entity);
        public abstract TEntity FromViewModel(TPutViewModel entity);


        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, TPutViewModel entity) => await base.Put(id, FromViewModel(entity)).ConfigureAwait(false);

        [HttpPost]
        public async Task<ActionResult<TEntity>> Post(TPutViewModel entity) => await base.Post(FromViewModel(entity)).ConfigureAwait(false);


        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        {
            var authorizedResult = await IsGetAuthorized().ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            var results = await Repository.AsQueryable().ToListAsync().ConfigureAwait(false);
            
            return Ok(results.Select(ToViewModel));
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TGetViewModel>> Get(int id)
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

            return Ok(ToViewModel(result));
        }
    }
}