using Microsoft.AspNetCore.Mvc;
using Method = GalaxyUML.Core.Models.Method;
using IMethodRepo = GalaxyUML.Data.Repositories.IMethodRepo;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/methods")]
    public class MethodController : ControllerBase
    {
        private readonly IMethodRepo _methodRepo;

        public MethodController(IMethodRepo methodRepo)
        {
            _methodRepo = methodRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var method = await _methodRepo.GetByIdAsync(id);
            if (method == null) return NotFound();
            return Ok(method);
        }

        [HttpGet("classBox/{idClassBox:guid}")]
        public async Task<IActionResult> GetByClassBoxAsync(Guid idClassBox)
        {
            var methods = await _methodRepo.GetByClassBoxAsync(idClassBox);
            return Ok(methods);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync(Guid idClassBox)
        {
            var methods = await _methodRepo.GetAllAsync();
            return Ok(methods);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Method method)
        {
            try
            {
                await _methodRepo.CreateAsync(method);
                return Ok(method);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] Method method)
        {
            try
            {
                await _methodRepo.UpdateAsync(id, method);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _methodRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}