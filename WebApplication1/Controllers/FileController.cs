using DomainLogic;
using DomainLogic.RequestModels;
using DomainLogic.Services;
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
    public class FileController : ControllerBase
    {
        private readonly FileService _service;
        private readonly IYandexDiskApi _client;

        public FileController(FileService service, IYandexDiskApi client)
        {
            _service = service;
            _client = client;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] GetFilesRequestModel request)
        {
            var id = User.Claims
                            .Where(c => c.Type == "userId")
                            .Select(c => c.Value)
                            .FirstOrDefault();

            request = request with { YandexUserId = id };

            var result = await _service.GetFilesByParentFolder(request);

            return Ok(result);
        }

        [HttpGet("getUrl")]
        public async Task<IActionResult> GetUrl([FromQuery] string path, [FromHeader(Name = "Authorization")] string authHeader)
        {
            if (string.IsNullOrEmpty(path))
                return BadRequest();

            var accessToken = authHeader.Replace("Bearer ", "");

            var result = await _client.ResourcesDownload(path, new YandexToken(accessToken, ""));

            return Ok(result);
        }
    }
}
