using DomainLogic;
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
    public class AuthController : ControllerBase
    {
        private readonly IYandexDiskApi _yandex;

        public AuthController(IYandexDiskApi yandex)
        {
            _yandex = yandex;
        }

        [HttpGet("getToken")]
        public async Task<ActionResult> GetToken(string code)
        {
            try
            {
                var token = await _yandex.GetToken(code);

                return Ok(new 
                { 
                    Success = true,
                    token.AccessToken,
                    token.RefreshToken
                });
            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    e.Message
                });
            }
        }

        [HttpGet("refreshToken")]
        public async Task<ActionResult> RefreshToken([FromHeader(Name = "refresh-token")]string refreshToken)
        {
            var yandexToken = new YandexToken("", refreshToken);

            await _yandex.RefreshToken(yandexToken);

            return Ok(yandexToken);
        }
    }
}
