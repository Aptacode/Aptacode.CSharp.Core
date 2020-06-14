using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public ActionResult<TViewModel> MapResponse<TEntity, TViewModel>(ServerResponse<TEntity> response)
        {
            if (!response.HasValue)
            {
                return BadRequest(response.Message);
            }

            var mappedValue = Mapper.Map<TViewModel>(response.Value);
            return ToActionResult(
                ServerResponse<TViewModel>.Create(response.StatusCode, response.Message, mappedValue));
        }

        public ActionResult<IEnumerable<TViewModel>> MapResponse<TEntity, TViewModel>(ServerResponse<IEnumerable<TEntity>> response)
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