using DomainLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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

        [HttpGet("authorize")]
        public async Task<IActionResult> Authorize(
            [FromQuery] string returnUrl,
            [FromServices] YandexAppOauthConfiguration yandexAppOauthConfiguration)
        {
            return Redirect($"{yandexAppOauthConfiguration.AuthorizationEndpoint}?" +
                $"client_id={yandexAppOauthConfiguration.ClientId}" +
                $"&response_type=code" +
                $"&redirect_uri={HttpUtility.UrlEncode(returnUrl)}");
        }

        [HttpPost("getToken")]
        public async Task<ActionResult> GetToken([FromForm] string code)
        {
            try
            {
                var token = await _yandex.GetToken(code);

                return Ok(new 
                {
                    oauth_token = token.OauthToken,
                    access_token = token.JwtToken,
                    refresh_token = token.RefreshToken
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
        public async Task<ActionResult> RefreshToken([FromHeader(Name = "refresh-token")] string refreshToken)
        {
            var token = new YandexToken(refreshToken: refreshToken);

            await _yandex.RefreshToken(token);

            return Ok(new
            {
                oauth_token = token.OauthToken,
                access_token = token.JwtToken,
                refresh_token = token.RefreshToken
            });
        }
    }
}
