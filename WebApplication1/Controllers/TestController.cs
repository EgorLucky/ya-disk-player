using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TestController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        
        [HttpGet("redirectToGetCode")]
        public async Task<IActionResult> RedirectToGetCode()
        {
             return Ok(User.Identity.Name);
        }

        [AllowAnonymous]
        [HttpGet("getTokenByCode")]
        public async Task<IActionResult> GetTokenByCode(string code)
        {
            

            return Ok("");
        }
    }
}
