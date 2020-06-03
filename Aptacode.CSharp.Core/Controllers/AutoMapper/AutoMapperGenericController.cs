using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Controllers.AutoMapper
{
    public abstract class
        AutoMapperGenericController<TViewModel, TEntity> : AutoMapperGenericController<TViewModel, TViewModel, TEntity>
        where TEntity : IEntity
    {
        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }

    public abstract class
        AutoMapperGenericController<TGetViewModel, TPutViewModel, TEntity> : GenericController<TEntity>
        where TEntity : IEntity
    {
        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            Mapper = mapper;
        }

        protected IMapper Mapper { get; }


        protected new virtual async Task<ActionResult<TGetViewModel>> Get(int id)
        {
            var response = await base.Get(id).ConfigureAwait(false);

            if (response.Value != null) return Ok(Mapper.Map<TGetViewModel>(response.Value));

            return response.Result;
        }

        protected new virtual async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        {
            var response = await base.Get().ConfigureAwait(false);

            if (response.Value != null) return Ok(response.Value.Select(r => Mapper.Map<TGetViewModel>(r)));

            return response.Result;
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Put(int id, TPutViewModel viewModel)
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            var response = await base.Put(id, entity).ConfigureAwait(false);

            if (response.Value != null) return Ok(Mapper.Map<TGetViewModel>(response.Value));

            return response.Result;
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Post(TPutViewModel viewModel)
        {
            var entity = Mapper.Map<TEntity>(viewModel);

            var response = await base.Post(entity).ConfigureAwait(false);
            if (response.Value != null) return Ok(Mapper.Map<TGetViewModel>(response.Value));

            return response.Result;
        }
    }
}