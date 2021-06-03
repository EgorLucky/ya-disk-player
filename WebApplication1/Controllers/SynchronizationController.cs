﻿using DomainLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            [FromHeader(Name = "Authorization")]string authHeader,
            [FromHeader(Name = "Refreshtoken")] string refreshToken)
        {
            var accessToken = authHeader.Replace("Bearer ", "");
            var id = User.Claims
                            .Where(c => c.Type == "userId")
                            .Select(c => c.Value)
                            .FirstOrDefault();

            var result = await _service.Start(id, accessToken, refreshToken);

            if (result.Success == false)
                return BadRequest(new {
                    result.ErrorMessage
                });

            return Ok(result.SynchronizationProcessId);
        }
    }
}