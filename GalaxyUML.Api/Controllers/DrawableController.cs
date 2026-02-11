using Microsoft.AspNetCore.Mvc;
using IDrawable = GalaxyUML.Core.Models.IDrawable;
using IDrawableRepo = GalaxyUML.Data.Repositories.IDrawableRepo;
using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Api.Controllers
{
    [ApiController]
    [Route("api/drawables")]
    public class DrawableController : ControllerBase
    {
        private readonly IDrawableRepo _drawableRepo;

        public DrawableController(IDrawableRepo drawableRepo)
        {
            _drawableRepo = drawableRepo;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var drawable = _drawableRepo.GetByIdAsync(id);
            if (drawable == null) return NotFound();
            return Ok(drawable);
        }

        [HttpGet("parent/{id:guid}")]
        public async Task<IActionResult> GetByParentAsync(Guid idParent)
        {
            var drawables = await _drawableRepo.GetByParentAsync(idParent);
            return Ok(drawables);
        }

        [HttpGet("parent/{id:guid}/type{type:int}")]
        public async Task<IActionResult> GetByParentTypeAsync(Guid idParent, ObjectType type)
        {
            var drawables = await _drawableRepo.GetByParentTypeAsync(idParent, type);
            return Ok(drawables);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var drawables = await _drawableRepo.GetAllAsync();
            return Ok(drawables);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] IDrawable drawable)
        {
            try
            {
                await _drawableRepo.CreateAsync(drawable);
                return Ok(drawable);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] IDrawable drawable)
        {
            try
            {
                await _drawableRepo.UpdateAsync(id, drawable);
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
                await _drawableRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}