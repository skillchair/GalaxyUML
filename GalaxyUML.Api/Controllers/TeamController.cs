using GalaxyUML.Core.Models;
using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController : ControllerBase
{
    private readonly TeamService _svc;
    public TeamsController(TeamService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamDto dto)
    {
        var id = await _svc.CreateAsync(dto.TeamName, dto.OwnerId);
        return Ok(id);
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(Guid id, [FromBody] JoinTeamDto dto)
    {
        await _svc.JoinAsync(id, dto.UserId, dto.JoinCode);
        return NoContent();
    }

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> Leave(Guid id, [FromBody] UserIdDto dto)
    {
        await _svc.LeaveAsync(id, dto.UserId);
        return NoContent();
    }

    [HttpPost("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleDto dto)
    {
        var role = Enum.Parse<RoleEnum>(dto.Role, true);
        await _svc.ChangeRoleAsync(id, dto.ActorId, dto.TargetUserId, role);
        return NoContent();
    }

    [HttpPost("{id:guid}/ban")]
    public async Task<IActionResult> Ban(Guid id, [FromBody] BanDto dto)
    {
        await _svc.BanAsync(id, dto.ActorId, dto.TargetUserId, dto.Reason);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromBody] UserIdDto dto)
    {
        await _svc.DeleteAsync(id, dto.UserId);
        return NoContent();
    }
}

public record CreateTeamDto(string TeamName, Guid OwnerId);
public record JoinTeamDto(Guid UserId, string JoinCode);
public record ChangeRoleDto(Guid ActorId, Guid TargetUserId, string Role);
public record BanDto(Guid ActorId, Guid TargetUserId, string? Reason);
public record UserIdDto(Guid UserId);
