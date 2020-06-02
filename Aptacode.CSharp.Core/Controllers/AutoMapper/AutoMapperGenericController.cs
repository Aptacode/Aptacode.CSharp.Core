using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Controllers
{
    public abstract class AutoMapperGenericController<TViewModel, TEntity> : AutoMapperGenericController<TViewModel, TViewModel, TEntity> where TEntity : IEntity
    {
        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, Mapper mapper) : base(unitOfWork, mapper)
        {

        }
    }

    public abstract class AutoMapperGenericController<TPutViewModel, TGetViewModel, TEntity> : GenericController<TEntity> where TEntity : IEntity
    {
        private Mapper _mapper { get; }

        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, Mapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Get(int id)
        {
            var result = await base.Get(id).ConfigureAwait(false);

            if (result.Value != null)
            {
                return Ok(_mapper.Map<TGetViewModel>(result.Value));
            }

            return result.Result;
        }

        protected virtual async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        {
            var result = await base.Get().ConfigureAwait(false);

            if (result.Value != null)
            {
                return Ok(result.Value.Select(r => _mapper.Map<TGetViewModel>(r)));
            }

            return result.Result;
        }

        protected virtual async Task<IActionResult> Put(int id, TPutViewModel viewModel)
        {
            var entity = _mapper.Map<TEntity>(viewModel);
            return await base.Put(id, entity).ConfigureAwait(false);
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Post(TPutViewModel viewModel)
        {
            var entity = _mapper.Map<TEntity>(viewModel);

            var result = await base.Post(entity).ConfigureAwait(false);
            if (result.Value != null)
            {
                return Ok(_mapper.Map<TGetViewModel>(result.Value));
            }

            return result.Result;
        }
    }
}