using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Controllers
{
    public abstract class
        ViewModelGenericController<TViewModel, TEntity> : ViewModelGenericController<TViewModel, TViewModel, TEntity>
        where TEntity : IEntity
    {
        protected ViewModelGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public abstract class ViewModelGenericController<TGetViewModel, TPutViewModel, TEntity> : GenericController<TEntity>
        where TEntity : IEntity
    {
        protected ViewModelGenericController(IGenericUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public abstract TGetViewModel ToViewModel(TEntity entity);
        public abstract TEntity FromViewModel(TPutViewModel entity);

        protected virtual async Task<ActionResult<TGetViewModel>> Put(int id, TPutViewModel entity)
        {
            var result = await base.Put(id, FromViewModel(entity)).ConfigureAwait(false);

            if (result.Value != null) return Ok(ToViewModel(result.Value));

            return result.Result;
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Post(TPutViewModel entity)
        {
            var result = await base.Post(FromViewModel(entity)).ConfigureAwait(false);

            if (result.Value != null) return Ok(ToViewModel(result.Value));

            return result.Result;
        }

        protected new virtual async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        {
            var response = await base.Get().ConfigureAwait(false);
            if (response.Value != null)
                return Ok(response.Value.Select(ToViewModel));
            return response.Result;
        }

        protected new virtual async Task<ActionResult<TGetViewModel>> Get(int id)
        {
            var response = await base.Get(id).ConfigureAwait(false);
            if (response.Value != null)
                return Ok(ToViewModel(response.Value));
            return response.Result;
        }
    }
}