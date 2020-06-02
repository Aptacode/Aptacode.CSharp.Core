using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aptacode.CSharp.Core.Controllers
{
    public abstract class ViewModelGenericController<TViewModel, TEntity> : ViewModelGenericController<TViewModel, TViewModel,TEntity>
        where TEntity : IEntity
    {
        protected ViewModelGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
    }

    public abstract class ViewModelGenericController<TPutViewModel, TGetViewModel, TEntity> : GenericController<TEntity> where TEntity : IEntity
    {
        protected ViewModelGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public abstract TGetViewModel ToViewModel(TEntity entity);
        public abstract TEntity FromViewModel(TPutViewModel entity);

        protected virtual async Task<IActionResult> Put(int id, TPutViewModel entity) => await base.Put(id, FromViewModel(entity)).ConfigureAwait(false);
        protected async Task<ActionResult<TEntity>> Post(TPutViewModel entity) => await base.Post(FromViewModel(entity)).ConfigureAwait(false);
        protected virtual async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        {
            var authorizedResult = await IsGetAuthorized().ConfigureAwait(false);
            if (!authorizedResult.Item1)
            {
                return authorizedResult.Item2;
            }

            var results = await Repository.AsQueryable().ToListAsync().ConfigureAwait(false);
            
            return Ok(results.Select(ToViewModel));
        }
        protected virtual async Task<ActionResult<TGetViewModel>> Get(int id)
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