﻿namespace TestOkur.WebApi.Application.OpticalForm
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Paramore.Darker;
    using TestOkur.Common;

    [Route("api/v1/optical-forms")]
    [Authorize(AuthorizationPolicies.Public)]
    public class OpticalFormsController : ControllerBase
    {
        private readonly IQueryProcessor _queryProcessor;

        public OpticalFormsController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor ?? throw new ArgumentNullException(nameof(queryProcessor));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<OpticalFormTypeReadModel>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 100000)]
        public async Task<IActionResult> Get()
        {
            return Ok(await _queryProcessor.ExecuteAsync(GetAllOpticalFormTypesQuery.Default));
        }
    }
}
