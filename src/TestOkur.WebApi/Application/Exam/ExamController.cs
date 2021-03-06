﻿namespace TestOkur.WebApi.Application.Exam
{
    using MassTransit;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Paramore.Brighter;
    using Paramore.Darker;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TestOkur.Common;
    using TestOkur.WebApi.Application.Exam.Commands;
    using TestOkur.WebApi.Application.Exam.Queries;

    [Route("api/v1/exams")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly IBus _bus;
        private readonly IQueryProcessor _queryProcessor;

        public ExamController(IBus bus, IAmACommandProcessor commandProcessor, IQueryProcessor queryProcessor)
        {
            _commandProcessor = commandProcessor ?? throw new ArgumentNullException(nameof(commandProcessor));
            _queryProcessor = queryProcessor;
            _bus = bus;
        }

        [HttpPost]
        [Authorize(AuthorizationPolicies.Customer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(CreateExamCommand command)
        {
            if (!User.IsInRole(Roles.Admin))
            {
                command.Shared = false;
            }

            await _commandProcessor.SendAsync(command);
            return Ok();
        }

        [HttpPost("re-evaluate")]
        [Authorize(AuthorizationPolicies.Private)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> ReEvaluateAsync()
        {
            var examIds = await _queryProcessor.ExecuteAsync(GetAllExamIdsQuery.Default);
            await _bus.Publish(new ReEvaluateMultipleExams(examIds));
            return Accepted();
        }

        [HttpGet]
        [Authorize(AuthorizationPolicies.Customer)]
        [ProducesResponseType(typeof(IReadOnlyCollection<ExamReadModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _queryProcessor.ExecuteAsync(new GetUserExamsQuery()));
        }

        [HttpDelete("{id}")]
        [Authorize(AuthorizationPolicies.Customer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _commandProcessor.SendAsync(new DeleteExamCommand(id, false));
            return Ok();
        }

        [HttpDelete("shared/{id}")]
        [Authorize(AuthorizationPolicies.Distributor)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSharedAsync(int id)
        {
            await _commandProcessor.SendAsync(new DeleteExamCommand(id, true));
            return Ok();
        }

        [HttpPut]
        [Authorize(AuthorizationPolicies.Customer)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditAsync(EditExamCommand command)
        {
            if (!User.IsInRole(Roles.Distributor))
            {
                command.Shared = false;
            }

            await _commandProcessor.SendAsync(command);
            return Ok();
        }
    }
}
