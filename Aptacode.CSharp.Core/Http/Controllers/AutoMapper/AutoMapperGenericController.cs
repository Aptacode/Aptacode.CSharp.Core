using System;
using System.Collections.Generic;
using System.Linq;
using Aptacode.CSharp.Common.Persistence.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Aptacode.CSharp.Core.Http.Controllers.AutoMapper
{
    /// <summary>
    /// Provides a collection of generic Http methods for querying &
    /// returning entities from an IRepository contained within the given IGenericUnitOfWork
    /// </summary>
    public abstract class AutoMapperGenericController : GenericController
    {
        protected AutoMapperGenericController(IGenericUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            Mapper = mapper ?? throw new NullReferenceException("IMapper was null");
        }

        #region Properties
        protected IMapper Mapper { get; }

        #endregion
        /// <summary>
        /// Converts the given ServerResponse<TEntity> into an ActionResult<TViewModel>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected ActionResult<TViewModel> ToActionResult<TEntity, TViewModel>(ServerResponse<TEntity> response)
        {
            if (!response.HasValue)
            {
                return BadRequest(response.Message);
            }

            var mappedValue = Mapper.Map<TViewModel>(response.Value);
            return ToActionResult(
                ServerResponse<TViewModel>.Create(response.StatusCode, response.Message, mappedValue));
        }

        /// <summary>
        /// Converts the given ServerResponse<IEnumerable<TEntity>> into an ActionResult<IEnumerable<TViewModel>>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected ActionResult<IEnumerable<TViewModel>> ToActionResult<TEntity, TViewModel>(ServerResponse<IEnumerable<TEntity>> response)
        {
            if (!response.HasValue)
            {
                return BadRequest(response.Message);
            }

            var mappedValue = response.Value.Select(r => Mapper.Map<TViewModel>(r));
            return ToActionResult(
                ServerResponse<IEnumerable<TViewModel>>.Create(response.StatusCode, response.Message, mappedValue));
        }
    }
}