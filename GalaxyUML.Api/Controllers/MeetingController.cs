using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[ApiController]
[Route("api/meetings")]
public class MeetingController : ControllerBase
{
    private readonly MeetingService _svc;
    public MeetingController(MeetingService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeetingDto dto)
    {
        try
        {
            var created = await _svc.CreateAsync(dto.TeamId, dto.OrganizerId);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(Guid id, [FromBody] UserIdDto dto)
    {
        await _svc.JoinAsync(id, dto.UserId);
        return NoContent();
    }

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> Leave(Guid id, [FromBody] UserIdDto dto)
    {
        await _svc.LeaveAsync(id, dto.UserId);
        return NoContent();
    }

    [HttpPost("{id:guid}/grant-draw")]
    public async Task<IActionResult> Grant(Guid id, [FromBody] GrantDrawDto dto)
    {
        await _svc.GrantDrawAsync(id, dto.ActorId, dto.TargetId, dto.CanDraw);
        return NoContent();
    }

    [HttpPost("{id:guid}/message")]
    public async Task<IActionResult> Message(Guid id, [FromBody] SendMessageDto dto)
    {
        await _svc.AddMessageAsync(id, dto.SenderId, dto.Content);
        return NoContent();
    }

    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(Guid id, [FromBody] UserIdDto dto)
    {
        try
        {
            await _svc.EndAsync(id, dto.UserId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}

public record CreateMeetingDto(Guid TeamId, Guid OrganizerId);
public record GrantDrawDto(Guid ActorId, Guid TargetId, bool CanDraw);
public record SendMessageDto(Guid SenderId, string Content);
