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
using WebApplication1.Controllers.Utils;

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
            var yandexUserId = User.GetUid();

            var result = await _service.GetFilesByParentFolder(request, yandexUserId);

            return Ok(result);
        }

        [HttpGet("getRandomFile")]
        public async Task<IActionResult> GetRandomFile([FromQuery] GetRandomFileRequestModel request)
        {
            var yandexUserId = User.GetUid();

            var result = await _service.GetRandomFile(request, yandexUserId);

            return Ok(result);
        }

        [HttpGet("getUrl")]
        public async Task<IActionResult> GetUrl(
            [FromQuery] string path,
            [FromHeader(Name = "oauth-token")] string oauthToken)
        {
            var result = await _client.ResourcesDownload(path, new YandexToken(oauthToken: oauthToken));

            return Ok(result);
        }
    }
}
