using System.Collections.Generic;
using System.Threading.Tasks;
using Aptacode.CSharp.Common.Persistence;
using Aptacode.CSharp.Common.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Http.Controllers.AutoMapper
{
    /// <summary>
    ///     Provides a collection of generic Http methods for querying &
    ///     returning entities from an IRepository contained within the given IGenericUnitOfWork
    /// </summary>
    /// <typeparam name="TGetViewModel"></typeparam>
    /// <typeparam name="TPutViewModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRestController<TKey, TGetViewModel, TPutViewModel, TEntity> : AutoMapperGenericController
        where TEntity : IEntity<TKey>
    {
        public GenericRestController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        #region HttpMethods

        [HttpPost("{id}")]
        public async Task<ActionResult<TGetViewModel>> Post(TKey id, [FromBody] TPutViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Post(id, model).ConfigureAwait(false);
            return ToActionResult<TEntity, TGetViewModel>(result);
        }

        [HttpPut]
        public async Task<ActionResult<TGetViewModel>> Put([FromBody] TPutViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Put< TKey, TEntity>(model).ConfigureAwait(false);
            return ToActionResult<TEntity, TGetViewModel>(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TGetViewModel>>> Get()
        {
            var result = await base.Get<TKey, TEntity>().ConfigureAwait(false);
            return ToActionResult<TEntity, TGetViewModel>(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TGetViewModel>> Get(TKey id)
        {
            var result = await base.Get<TKey, TEntity>(id).ConfigureAwait(false);
            return ToActionResult<TEntity, TGetViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(TKey id)
        {
            var result = await base.Delete<TKey, TEntity>(id).ConfigureAwait(false);
            return ToActionResult(result);
        }

        #endregion
    }

    /// <summary>
    ///     Provides a collection of generic Http methods for querying &
    ///     returning entities from an IRepository contained within the given IGenericUnitOfWork
    /// </summary>
    /// <typeparam name="TGetViewModel"></typeparam>
    /// <typeparam name="TPutViewModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRestController<TKey, TViewModel, TEntity> : AutoMapperGenericController
        where TEntity : IEntity<TKey>
    {
        public GenericRestController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        #region HttpMethods

        [HttpPost("{id}")]
        public async Task<ActionResult<TViewModel>> Post(TKey id, [FromBody] TViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Post(id, model).ConfigureAwait(false);
            return ToActionResult<TEntity, TViewModel>(result);
        }

        [HttpPut]
        public async Task<ActionResult<TViewModel>> Put([FromBody] TViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            var result = await base.Put< TKey, TEntity>(model).ConfigureAwait(false);
            return ToActionResult<TEntity, TViewModel>(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TViewModel>>> Get()
        {
            var result = await base.Get<TKey, TEntity>().ConfigureAwait(false);
            return ToActionResult<TEntity, TViewModel>(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TViewModel>> Get(TKey id)
        {
            var result = await base.Get<TKey, TEntity>(id).ConfigureAwait(false);
            return ToActionResult<TEntity, TViewModel>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(TKey id)
        {
            var result = await base.Delete<TKey, TEntity>(id).ConfigureAwait(false);
            return ToActionResult(result);
        }

        #endregion
    }
}