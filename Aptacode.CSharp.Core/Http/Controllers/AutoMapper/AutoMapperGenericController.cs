using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Aptacode.CSharp.Common.Persistence;
using Aptacode.CSharp.Common.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Http.Controllers.AutoMapper
{
    public abstract class AutoMapperGenericController : GenericController
    {
        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            Mapper = mapper ?? throw new NullReferenceException("IMapper was null");
        }

        #region Properties

        protected IMapper Mapper { get; }

        #endregion

        //Maps an ActionResult<TEntity> to An ActionResult<TViewModel>
        private ActionResult<TViewModel> MapResponse<TEntity, TViewModel>(ServerResponse<TEntity> response)
        {
            if (!response.HasValue)
            {
                return BadRequest(response.Message);
            }

            var mappedValue = Mapper.Map<TViewModel>(response.Value);
            return ToActionResult(
                ServerResponse<TViewModel>.Create(response.StatusCode, response.Message, mappedValue));
        }

        //Maps an ActionResult<IEnumerable<TEntity>> to An ActionResult<IEnumerable<TViewModel>>
        private ActionResult<IEnumerable<TViewModel>> MapResponse<TEntity, TViewModel>(
            ServerResponse<IEnumerable<TEntity>> response)
        {
            if (!response.HasValue)
            {
                return BadRequest(response.Message);
            }

            var mappedValue = response.Value.Select(r => Mapper.Map<TViewModel>(r));
            return ToActionResult(
                ServerResponse<IEnumerable<TViewModel>>.Create(response.StatusCode, response.Message, mappedValue));
        }

        #region HttpMethods

        protected virtual async Task<ActionResult<TViewModel>> Get<TViewModel, TEntity>(int id,
            Validator<int> validator = null) where TEntity : IEntity
        {
            var response = await base.Get<TEntity>(id, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(response);
        }
        
        protected virtual async Task<ActionResult<IEnumerable<TViewModel>>> Get<TViewModel, TEntity>(
            Expression<Func<TEntity, bool>> queryExpression = null, Validator validator = null)
            where TEntity : IEntity
        {
            var response = await base.Get(queryExpression, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(response);
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Post<TGetViewModel, TPostViewModel, TEntity>(int id,
            TPostViewModel viewModel, Validator<TEntity> validator = null)
            where TEntity : IEntity
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            var response = await base.Post(id, entity, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(response);
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Put<TGetViewModel, TPostViewModel, TEntity>(
            TPostViewModel viewModel, Validator<TEntity> validator = null)
            where TEntity : IEntity
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            var response = await base.Put(entity, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(response);
        }

        protected virtual async Task<ActionResult<bool>> Delete<TEntity>(int id) where TEntity : IEntity
        {
            var response = await base.Delete<TEntity>(id).ConfigureAwait(false);
            return ToActionResult(response);
        }

        #endregion
    }
}