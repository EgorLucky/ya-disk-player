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
    public class IgnorePathController : ControllerBase
    {
        private readonly IgnorePathService _service;

        public IgnorePathController(IgnorePathService service)
        {
            _service = service;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(List<string> paths)
        {
            var id = User.Claims
                            .Where(c => c.Type == "userId")
                            .Select(c => c.Value)
                            .FirstOrDefault();

            var result = await _service.Add(paths, id);

            if (result.Success == false)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(int take, int page, string search)
        {
            var id = User.Claims
                            .Where(c => c.Type == "userId")
                            .Select(c => c.Value)
                            .FirstOrDefault();

            var result = await _service.Get(take, page, search, id);

            return Ok(result);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(List<string> paths)
        {
            var id = User.Claims
                            .Where(c => c.Type == "userId")
                            .Select(c => c.Value)
                            .FirstOrDefault();

            var result = await _service.Delete(paths, id);

            if (result.Success == false)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }
    }
}
