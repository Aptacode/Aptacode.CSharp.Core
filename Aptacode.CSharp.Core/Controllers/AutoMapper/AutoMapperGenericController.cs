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
        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            Mapper = mapper;
        }

        protected IMapper Mapper { get; }

        protected virtual async Task<ActionResult<TViewModel>> Get<TViewModel, TEntity>(int id,
            Func<int, Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            var response = await base.Get<TEntity>(id, validator).ConfigureAwait(false);

            if (response.Value != null) return Ok(Mapper.Map<TViewModel>(response.Value));

            return response.Result;
        }

        protected virtual async Task<ActionResult<IEnumerable<TViewModel>>> Get<TViewModel, TEntity>(
            Func<Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            var response = await base.Get<TEntity>(validator).ConfigureAwait(false);

            if (response.Value != null) return Ok(response.Value.Select(r => Mapper.Map<TViewModel>(r)));

            return response.Result;
        }

        protected virtual async Task<ActionResult<IEnumerable<TViewModel>>> Get<TViewModel, TEntity>(Expression<Func<TEntity, bool>> queryExpression, Func<Task<(bool, StatusCodeResult)>> validator = null) where TEntity : IEntity
        {
            var response = await base.Get(queryExpression, validator).ConfigureAwait(false);

            if (response.Value != null) return Ok(response.Value.Select(r => Mapper.Map<TViewModel>(r)));

            return response.Result;
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Post<TGetViewModel, TPostViewModel, TEntity>(int id,
            TPostViewModel viewModel, Func<TEntity, Task<(bool, StatusCodeResult)>> validator = null)
            where TEntity : IEntity
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            var response = await base.Post(id, entity, validator).ConfigureAwait(false);

            if (response.Value != null) return Ok(Mapper.Map<TGetViewModel>(response.Value));

            return response.Result;
        }

        protected virtual async Task<ActionResult<TGetViewModel>> Put<TGetViewModel, TPostViewModel, TEntity>(
            TPostViewModel viewModel, Func<TEntity, Task<(bool, StatusCodeResult)>> validator = null)
            where TEntity : IEntity
        {
            var entity = Mapper.Map<TEntity>(viewModel);

            var response = await base.Put(entity, validator).ConfigureAwait(false);
            if (response.Value != null) return Ok(Mapper.Map<TGetViewModel>(response.Value));

            return response.Result;
        }
    }
}