using System.Collections.Generic;
using System.Threading.Tasks;
using Aptacode.CSharp.Common.Persistence;
using Aptacode.CSharp.Common.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Http.Controllers.AutoMapper
{
    public class GenericRestController<TGetViewModel, TPutViewModel, TEntity> : AutoMapperGenericController
        where TEntity : IEntity
    {
        public GenericRestController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        #region HttpMethods

        [HttpPost("{id}")]
        public async Task<ActionResult<TGetViewModel>> Post(int id, [FromBody] TPutViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Post(id, model).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(result);
        }

        [HttpPut]
        public async Task<ActionResult<TGetViewModel>> Put([FromBody] TPutViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Put(model).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        { 
            var result = await base.Get<TEntity>().ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TGetViewModel>> Get(int id)
        {
            var result = await base.Get<TEntity>(id).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await base.Delete<TEntity>(id).ConfigureAwait(false);
            return ToActionResult(result);
        }

        #endregion
    }


    public class GenericRestController<TViewModel, TEntity> : AutoMapperGenericController
        where TEntity : IEntity
    {
        public GenericRestController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        #region HttpMethods

        [HttpPost("{id}")]
        public async Task<ActionResult<TViewModel>> Post(int id, [FromBody] TViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Post(id, model).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(result);
        }

        [HttpPut]
        public async Task<ActionResult<TViewModel>> Put([FromBody] TViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Put(model).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TViewModel>>> Get()
        {
            var result = await base.Get<TEntity>().ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TViewModel>> Get(int id)
        {
            var result = await base.Get<TEntity>(id).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await base.Delete<TEntity>(id).ConfigureAwait(false);
            return ToActionResult(result);
        }

        #endregion
    }
}