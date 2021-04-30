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
    public class TestController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public IActionResult RedirectToGetCode()
        {

            var redirectUrl = @$"https://oauth.yandex.ru/authorize?response_type=code&client_id={Environment.GetEnvironmentVariable("yandexAppId")}
  {/*[&device_id =< идентификатор устройства >]
  &device_name =< имя устройства */""}
  &redirect_uri={"http://localhost:3000/verification_code"}
  {/*[&login_hint =< имя пользователя или электронный адрес >]
  [&scope =< запрашиваемые необходимые права >]
  [&optional_scope =< запрашиваемые опциональные права >]*/""}
  &force_confirm=yes
  &state={Guid.NewGuid()}";
             return Redirect(redirectUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetTokenByCode(string code)
        {
            

            return Ok("");
        }
    }
}
