﻿namespace TestOkur.Notification.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TestOkur.Common;
    using TestOkur.Infrastructure.Mvc.Extensions;
    using TestOkur.Notification.Dtos;
    using TestOkur.Notification.Infrastructure.Data;
    using TestOkur.Notification.Models;

    [Route("api/v1/sms")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsRepository _smsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SmsController(ISmsRepository smsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _smsRepository = smsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize(AuthorizationPolicies.Customer)]
        [ProducesResponseType(typeof(IEnumerable<UserSmsModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSmsesAsync()
        {
            var smses = await _smsRepository.GetUserSmsesAsync(_httpContextAccessor.GetUserId());
            return Ok(smses.Select(s => new UserSmsModel(s)));
        }

        [HttpGet("today")]
        [Authorize(AuthorizationPolicies.Admin)]
        [ProducesResponseType(typeof(IEnumerable<Sms>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodaysSmsesAsync()
        {
            return Ok(await _smsRepository.GetTodaysSmsesAsync());
        }

        [HttpGet("{userId}/logs")]
        [Authorize(AuthorizationPolicies.Admin)]
        [ProducesResponseType(typeof(IEnumerable<SmsLog>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSmsLogsAsync(int userId)
        {
            return Ok(await _smsRepository.GetUserSmsLogsAsync(userId));
        }
    }
}
