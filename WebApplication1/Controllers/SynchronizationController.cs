using DomainLogic.RequestModels;
using DomainLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Controllers.Utils;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "RegistredUser")]
    public class SynchronizationController : ControllerBase
    {
        private readonly SynchronizationService _service;

        public SynchronizationController(SynchronizationService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(
            [FromHeader(Name = "oauth-token")]string oauthToken,
            [FromHeader(Name = "refresh-token")] string refreshToken)
        {
            var userId = User.GetUid();

            var result = await _service.Start(userId, oauthToken, refreshToken);

            if (result.Success == false)
                return BadRequest(new {
                    result.ErrorMessage
                });

            return Ok(result.SynchronizationProcessId);
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop(Guid? synchronizationProcessId)
        {
            var userId = User.GetUid();

            var result = await _service.Stop(userId, synchronizationProcessId);

            if (result.Success == false)
                return BadRequest(new
                {
                    result.ErrorMessage
                });

            return Ok();
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get([FromQuery]GetSynchronizationProcessesRequestModel request)
        {
            var userId = User.GetUid();

            var result = await _service.Get(request, userId);

            return Ok(result);
        }
    }
}
