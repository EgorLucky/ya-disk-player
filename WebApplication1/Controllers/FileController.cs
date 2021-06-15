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

        public FileController(FileService service)
        {
            _service = service;
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
    }
}
