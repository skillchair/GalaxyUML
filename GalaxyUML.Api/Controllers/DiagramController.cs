using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[ApiController]
[Route("api/diagram")]
public class DiagramController : ControllerBase
{
    private readonly DiagramService _svc;
    public DiagramController(DiagramService svc) => _svc = svc;

    [HttpPost("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveDto dto)
    { await _svc.MoveAsync(id, dto.Dx, dto.Dy); return NoContent(); }

    [HttpPost("{id:guid}/resize")]
    public async Task<IActionResult> Resize(Guid id, [FromBody] ResizeDto dto)
    { await _svc.ResizeAsync(id, dto.Width, dto.Height); return NoContent(); }

    [HttpPost("{id:guid}/text")]
    public async Task<IActionResult> EditText(Guid id, [FromBody] EditTextDto dto)
    { await _svc.EditTextAsync(id, dto.Content, dto.FontSize, dto.Color, dto.Format); return NoContent(); }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    { await _svc.DeleteAsync(id); return NoContent(); }
}

public record MoveDto(int Dx, int Dy);
public record ResizeDto(int Width, int Height);
public record EditTextDto(string Content, int FontSize, string Color, string? Format);
