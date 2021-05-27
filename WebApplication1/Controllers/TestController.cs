using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YandexDiskPlayerLibrary;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TestController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        [HttpGet("")]
        public async Task<IActionResult> Test([FromHeader(Name = "Authorization")] string authHeader)
        {
            var accessToken = authHeader.Replace("Bearer ", "");
            var yaClient = new YandexDiskPlayerApi(_httpClientFactory, accessToken);

            var res = await yaClient.LoadFiles();
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("getTokenByCode")]
        public async Task<IActionResult> GetTokenByCode(string code)
        {
            

            return Ok("");
        }
    }
}
