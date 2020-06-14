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
        public async Task<ActionResult<TGetViewModel>> Post(int id, [FromBody] TPutViewModel viewModel) =>
            await base.Post<TGetViewModel, TPutViewModel, TEntity>(id, viewModel).ConfigureAwait(false);

        [HttpPut]
        public async Task<ActionResult<TGetViewModel>> Put([FromBody] TPutViewModel viewModel) =>
            await base.Put<TGetViewModel, TPutViewModel, TEntity>(viewModel).ConfigureAwait(false);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TGetViewModel>>> Get() =>
            await base.Get<TGetViewModel, TEntity>().ConfigureAwait(false);

        [HttpGet("{id}")]
        public async Task<ActionResult<TGetViewModel>> Get(int id) =>
            await base.Get<TGetViewModel, TEntity>(id).ConfigureAwait(false);

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id) => await base.Delete<TEntity>(id).ConfigureAwait(false);

        #endregion
    }


    public class GenericRestController<TViewModel, TEntity> : AutoMapperGenericController
        where TEntity : IEntity
    {
        public GenericRestController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        #region HttpMethods

        [HttpPost("{id}")]
        public async Task<ActionResult<TViewModel>> Post(int id, [FromBody] TViewModel viewModel) =>
            await base.Post<TViewModel, TViewModel, TEntity>(id, viewModel).ConfigureAwait(false);

        [HttpPut]
        public async Task<ActionResult<TViewModel>> Put([FromBody] TViewModel viewModel) =>
            await base.Put<TViewModel, TViewModel, TEntity>(viewModel).ConfigureAwait(false);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TViewModel>>> Get() =>
            await base.Get<TViewModel, TEntity>().ConfigureAwait(false);

        [HttpGet("{id}")]
        public async Task<ActionResult<TViewModel>> Get(int id) =>
            await base.Get<TViewModel, TEntity>(id).ConfigureAwait(false);

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id) => await base.Delete<TEntity>(id).ConfigureAwait(false);

        #endregion
    }
}