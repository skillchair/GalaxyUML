// exposes authenticated team operations.

using GalaxyUML.Api.Security;
using GalaxyUML.Core.Models;
using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/teams")]
public class TeamsController(TeamService teams, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamDto dto)
    {
        try
        {
            var team = await teams.CreateAsync(dto.TeamName, currentUser.Id);
            return Ok(team);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(Guid id, [FromBody] JoinTeamDto dto)
    {
        try
        {
            await teams.JoinAsync(id, currentUser.Id, dto.JoinCode);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> FindByCode(string code)
    {
        var team = await teams.FindByCodeAsync(code);
        return team is null ? NotFound() : Ok(team);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserTeams()
    {
        try
        {
            var userTeams = await teams.GetUserTeamsAsync(currentUser.Id);
            return Ok(userTeams);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetMembers(Guid id)
    {
        try
        {
            var members = await teams.GetMembersAsync(id, currentUser.Id);
            return Ok(members);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("join-by-code")]
    public async Task<IActionResult> JoinByCode([FromBody] JoinByCodeDto dto)
    {
        try
        {
            var team = await teams.JoinByCodeAsync(currentUser.Id, dto.JoinCode);
            return Ok(team);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> Leave(Guid id)
    {
        try
        {
            await teams.LeaveAsync(id, currentUser.Id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleDto dto)
    {
        if (!Enum.TryParse<RoleEnum>(dto.Role, true, out var role))
        {
            return BadRequest(new { error = "Unknown team role" });
        }

        try
        {
            await teams.ChangeRoleAsync(id, currentUser.Id, dto.TargetUserId, role);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/ban")]
    public async Task<IActionResult> Ban(Guid id, [FromBody] BanDto dto)
    {
        try
        {
            await teams.BanAsync(id, currentUser.Id, dto.TargetUserId, dto.Reason);
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
        try
        {
            await teams.DeleteAsync(id, currentUser.Id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record CreateTeamDto(string TeamName);
public record JoinTeamDto(string JoinCode);
public record JoinByCodeDto(string JoinCode);
public record ChangeRoleDto(Guid TargetUserId, string Role);
public record BanDto(Guid TargetUserId, string? Reason);
