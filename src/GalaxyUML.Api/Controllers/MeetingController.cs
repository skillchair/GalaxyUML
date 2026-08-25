// exposes authenticated meeting operations.

using GalaxyUML.Api.Hubs;
using GalaxyUML.Api.Security;
using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GalaxyUML.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/meetings")]
public class MeetingController(MeetingService meetings, IHubContext<DiagramHub> hubContext, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeetingDto dto)
    {
        try
        {
            var created = await meetings.CreateAsync(dto.TeamId, currentUser.Id);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join(Guid id)
    {
        try
        {
            await meetings.JoinAsync(id, currentUser.Id);
            await hubContext.Clients.Group(id.ToString()).SendAsync("ParticipantsUpdated", id);
            return NoContent();
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
            await meetings.LeaveAsync(id, currentUser.Id);
            await hubContext.Clients.Group(id.ToString()).SendAsync("ParticipantsUpdated", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/grant-draw")]
    public async Task<IActionResult> Grant(Guid id, [FromBody] GrantDrawDto dto)
    {
        try
        {
            await meetings.GrantDrawAsync(id, currentUser.Id, dto.TargetId, dto.CanDraw);
            await hubContext.Clients.Group(id.ToString()).SendAsync("DrawPermissionChanged", dto.TargetId, dto.CanDraw);
            await hubContext.Clients.Group(id.ToString()).SendAsync("ParticipantsUpdated", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/message")]
    public async Task<IActionResult> Message(Guid id, [FromBody] SendMessageDto dto)
    {
        try
        {
            var msg = await meetings.AddMessageAsync(id, currentUser.Id, dto.Content);
            await hubContext.Clients.Group(id.ToString())
                .SendAsync("ChatMessageReceived", msg.Id, msg.SenderId, msg.SenderUsername, msg.Content, msg.SentAtUtc);
            return Ok(msg);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id)
    {
        try
        {
            var messagesList = await meetings.GetMessagesAsync(id, currentUser.Id);
            return Ok(messagesList);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(Guid id)
    {
        try
        {
            await meetings.EndAsync(id, currentUser.Id);
            await hubContext.Clients.Group(id.ToString()).SendAsync("MeetingEnded", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/participants")]
    public async Task<IActionResult> Participants(Guid id)
    {
        try
        {
            var participants = await meetings.GetParticipantsAsync(id);
            return Ok(participants);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("by-team/{teamId:guid}/active")]
    public async Task<IActionResult> ActiveMeeting(Guid teamId)
    {
        var meeting = await meetings.GetByTeamIfActiveAsync(teamId);
        return meeting is null ? NoContent() : Ok(meeting);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await meetings.DeleteAsync(id, currentUser.Id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record CreateMeetingDto(Guid TeamId);
public record GrantDrawDto(Guid TargetId, bool CanDraw);
public record SendMessageDto(string Content);
