using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Aptacode.CSharp.Utilities.Persistence;
using Aptacode.CSharp.Utilities.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Controllers.AutoMapper
{
    public abstract class AutoMapperGenericController : GenericController
    {
        #region Properties
        protected IMapper Mapper { get; }

        #endregion

        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            Mapper = mapper ?? throw new NullReferenceException("IMapper was null");
        }

        #region HttpMethods

        protected virtual async Task<ActionResult<TViewModel>> Get<TViewModel, TEntity>(int id, Func<int, Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            var response = await base.Get<TEntity>(id, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(response);
        }

        protected virtual async Task<ActionResult<IEnumerable<TViewModel>>> Get<TViewModel, TEntity>(
            Func<Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            var response = await base.Get<TEntity>(validator).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(response);
        }

        protected virtual async Task<ActionResult<IEnumerable<TViewModel>>> Get<TViewModel, TEntity>(
            Expression<Func<TEntity, bool>> queryExpression, Func<Task<(bool, StatusCodeResult)>> validator = null)
            where TEntity : IEntity
        {
            var response = await base.Get(queryExpression, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TViewModel>(response);
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Post<TGetViewModel, TPostViewModel, TEntity>(int id,
            TPostViewModel viewModel, Func<TEntity, Task<(bool, StatusCodeResult)>> validator = null)
            where TEntity : IEntity
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            var response = await base.Post(id, entity, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(response);
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Put<TGetViewModel, TPostViewModel, TEntity>(
            TPostViewModel viewModel, Func<TEntity, Task<(bool, StatusCodeResult)>> validator = null)
            where TEntity : IEntity
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            var response = await base.Put(entity, validator).ConfigureAwait(false);
            return MapResponse<TEntity, TGetViewModel>(response);
        }

        #endregion

        //Maps an ActionResult<TEntity> to An ActionResult<TViewModel>
        private ActionResult<TViewModel> MapResponse<TEntity, TViewModel>(ActionResult<TEntity> response)
        {
            return response.Value != null ? Ok(Mapper.Map<TViewModel>(response.Value)) : response.Result;
        }

        //Maps an ActionResult<IEnumerable<TEntity>> to An ActionResult<IEnumerable<TViewModel>>
        private ActionResult<IEnumerable<TViewModel>> MapResponse<TEntity, TViewModel>(ActionResult<IEnumerable<TEntity>> response)
        {
            return response.Value != null ? Ok(response.Value.Select(r => Mapper.Map<TViewModel>(r))) : response.Result;
        }
    }
}