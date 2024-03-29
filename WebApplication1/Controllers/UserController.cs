﻿using DomainLogic.RequestModels;
using DomainLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(
            UserService userService
            )
        {
            _userService = userService;
        }

        [HttpPost("createFirstUser")]
        public async Task<IActionResult> CreateFirstUser()
        {
            var userClaims = User
                            .Claims
                            .ToDictionary(c => c.Type);

            var email = userClaims[ClaimTypes.Email].Value;
            var yandexId = userClaims["uid"].Value;

            var result = await _userService.TryCreateFirstUser(yandexId, email);

            if (result == false)
                return BadRequest();

            return Ok();
        }

        [HttpGet("registerByInvite")]
        public async Task<IActionResult> RegisterByInvite(Guid? inviteId)
        {
            if (inviteId == null)
                return BadRequest();

            var userClaims = User
                            .Claims
                            .ToDictionary(c => c.Type);

            var email = userClaims[ClaimTypes.Email].Value;
            var yandexId = userClaims["uid"].Value;

            var registrationResult = await _userService.RegisterByInvite(inviteId.Value, yandexId, email);

            if (registrationResult.Success == false)
                return BadRequest(new
                {
                    registrationResult.ErrorMessage
                });

            return Ok(registrationResult.User);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("createUserInvite")]
        public async Task<IActionResult> CreateUserInvite(CreateUserInviteRequest request)
        {
            var result = await _userService.CreateUserInvite(request);

            if(result.Success == false)
            {
                return BadRequest(new
                {
                    result.ErrorMessage
                });
            }

            return Ok(result.InviteId);
        }

        [Authorize(Policy = "RegistredUser")]
        [HttpGet("getUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            var result = await _userService.GetUserByEmail(email.Value);

            return Ok(result);
        }


        /*
         var userClaims = User
                            .Claims
                            .ToDictionary(c => c.Type);

            var user = new User(
                YandexId: userClaims["userId"].Value,
                Email: userClaims["default_email"].Value,
                Firstname: userClaims["first_name"].Value,
                Lastname: userClaims["last_name"].Value,
                Login: userClaims["login"].Value,
                Sex: userClaims["sex"].Value,
                InviteId: inviteId.Value
            );
         */
    }
}
