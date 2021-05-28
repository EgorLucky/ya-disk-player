using DomainLogic;
using DomainLogic.Entities;
using DomainLogic.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("registerByInvite")]
        public async Task<IActionResult> RegisterByInvite(Guid? inviteId)
        {
            if (inviteId == null)
                return BadRequest();

            var userClaims = User
                            .Claims
                            .ToDictionary(c => c.Type);

            var email = userClaims["default_email"].Value;

            var registrationResult = await _userService.RegisterByInvite(inviteId.Value, email);

            if (registrationResult.Success == false)
                return BadRequest(new
                {
                    registrationResult.ErrorMessage
                });

            return Ok(registrationResult.User);
        }

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
