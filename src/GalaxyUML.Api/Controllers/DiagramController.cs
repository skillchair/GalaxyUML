// exposes authenticated diagram operations and realtime notifications.

using GalaxyUML.Api.Hubs;
using GalaxyUML.Api.Security;
using GalaxyUML.Core.Models;
using GalaxyUML.Core.Services;
using GalaxyUML.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/diagram")]
public class DiagramController(
    DiagramService diagrams,
    IHubContext<DiagramHub> hubContext,
    AppDbContext db,
    ICurrentUser currentUser) : ControllerBase
{
    [HttpPost("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveDto dto)
    {
        if (!await CanCurrentUserDrawElementAsync(id))
        {
            return Forbid();
        }

        await diagrams.MoveAsync(id, dto.Dx, dto.Dy);

        var meetingId = await GetMeetingIdForElementAsync(id);
        if (meetingId.HasValue)
        {
            await hubContext.Clients.Group(meetingId.Value.ToString())
                .SendAsync("ElementMoved", id, dto.Dx, dto.Dy);
        }

        return NoContent();
    }

    [HttpPost("{id:guid}/resize")]
    public async Task<IActionResult> Resize(Guid id, [FromBody] ResizeDto dto)
    {
        if (!await CanCurrentUserDrawElementAsync(id))
        {
            return Forbid();
        }

        await diagrams.ResizeAsync(id, dto.Width, dto.Height);
        return NoContent();
    }

    [HttpPost("{id:guid}/text")]
    public async Task<IActionResult> EditText(Guid id, [FromBody] EditTextDto dto)
    {
        if (!await CanCurrentUserDrawElementAsync(id))
        {
            return Forbid();
        }

        await diagrams.EditTextAsync(id, dto.Content, dto.FontSize, dto.Color, dto.Format);
        return NoContent();
    }

    [HttpPost("{id:guid}/class-box")]
    public async Task<IActionResult> AddClassBox(Guid id, [FromBody] AddClassBoxDto dto)
    {
        try
        {
            if (!await CanCurrentUserDrawDiagramAsync(id))
            {
                return Forbid();
            }

            var elementId = await diagrams.AddClassBoxAsync(
                id,
                dto.X1,
                dto.Y1,
                dto.X2,
                dto.Y2,
                dto.Attributes,
                dto.Methods);

            var meetingId = await GetMeetingIdForDiagramAsync(id);
            if (meetingId.HasValue)
            {
                await hubContext.Clients.Group(meetingId.Value.ToString())
                    .SendAsync("ClassBoxAdded", elementId, dto.X1, dto.Y1, dto.X2, dto.Y2);
            }

            return Ok(new { id = elementId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/line")]
    public async Task<IActionResult> AddLine(Guid id, [FromBody] AddLineDto dto)
    {
        try
        {
            if (!await CanCurrentUserDrawDiagramAsync(id))
            {
                return Forbid();
            }

            var elementId = await diagrams.AddLineAsync(
                id,
                dto.StartBoxId,
                dto.EndBoxId,
                dto.MiddleText,
                dto.Text1,
                dto.Text2);

            var meetingId = await GetMeetingIdForDiagramAsync(id);
            if (meetingId.HasValue)
            {
                var startBox = await db.Boxes.FirstOrDefaultAsync(box => box.Id == dto.StartBoxId);
                var endBox = await db.Boxes.FirstOrDefaultAsync(box => box.Id == dto.EndBoxId);

                if (startBox is not null && endBox is not null)
                {
                    await hubContext.Clients.Group(meetingId.Value.ToString())
                        .SendAsync(
                            "LineAdded",
                            elementId,
                            dto.StartBoxId,
                            dto.EndBoxId,
                            (startBox.X1 + startBox.X2) / 2d,
                            (startBox.Y1 + startBox.Y2) / 2d,
                            (endBox.X1 + endBox.X2) / 2d,
                            (endBox.Y1 + endBox.Y2) / 2d);
                }
            }

            return Ok(new { id = elementId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await CanCurrentUserDrawElementAsync(id))
        {
            return Forbid();
        }

        await diagrams.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/clear")]
    public async Task<IActionResult> ClearBoard(Guid id)
    {
        if (!await CanCurrentUserDrawDiagramAsync(id))
        {
            return Forbid();
        }

        var elements = await db.DiagramElements
            .Where(element => element.ParentId == id && element.ObjectType != 0)
            .ToListAsync();

        if (elements.Count > 0)
        {
            db.DiagramElements.RemoveRange(elements);
            await db.SaveChangesAsync();

            var meetingId = await GetMeetingIdForDiagramAsync(id);
            if (meetingId.HasValue)
            {
                await hubContext.Clients.Group(meetingId.Value.ToString())
                    .SendAsync("BoardCleared", id);
            }
        }

        return Ok(new { deletedCount = elements.Count });
    }

    [HttpGet("{id:guid}/elements")]
    public async Task<IActionResult> GetElements(Guid id)
    {
        if (!await CanCurrentUserViewDiagramAsync(id))
        {
            return Forbid();
        }

        var diagramExists = await db.Diagrams.AnyAsync(diagram => diagram.Id == id);
        if (!diagramExists)
        {
            return NotFound();
        }

        var boxes = await db.Boxes
            .Where(box => box.ParentId == id && box.ObjectType == ObjectType.Box)
            .Select(box => new
            {
                id = box.Id,
                x1 = (int)box.X1,
                y1 = (int)box.Y1,
                x2 = (int)box.X2,
                y2 = (int)box.Y2
            })
            .ToListAsync();

        var classBoxes = await db.ClassBoxes
            .Where(box => box.ParentId == id)
            .Select(box => new
            {
                id = box.Id,
                x1 = (int)box.X1,
                y1 = (int)box.Y1,
                x2 = (int)box.X2,
                y2 = (int)box.Y2,
                attributes = box.Attributes.Select(a => a.Name).ToList(),
                methods = box.Methods.Select(m => m.Signature).ToList()
            })
            .ToListAsync();

        var texts = await db.Texts
            .Where(text => text.ParentId == id)
            .Select(text => new
            {
                id = text.Id,
                x1 = (int)text.X1,
                y1 = (int)text.Y1,
                x2 = (int)text.X2,
                y2 = (int)text.Y2,
                content = text.Content,
                fontSize = text.FontSize,
                color = text.Color,
                format = text.Format
            })
            .ToListAsync();

        var lines = await db.Lines
            .Where(line => line.ParentId == id)
            .Select(line => new
            {
                id = line.Id,
                startBoxId = line.StartBoxId,
                endBoxId = line.EndBoxId,
                x1 = line.X1,
                y1 = line.Y1,
                x2 = line.X2,
                y2 = line.Y2,
                middleText = line.MiddleText,
                text1 = line.Text1,
                text2 = line.Text2
            })
            .ToListAsync();

        return Ok(new { boxes, classBoxes, texts, lines });
    }

    private async Task<Guid?> GetMeetingIdForDiagramAsync(Guid diagramId)
    {
        return await db.Diagrams
            .Where(diagram => diagram.Id == diagramId)
            .Select(diagram => diagram.MeetingId)
            .FirstOrDefaultAsync();
    }

    private async Task<Guid?> GetMeetingIdForElementAsync(Guid elementId)
    {
        var diagramId = await db.DiagramElements
            .Where(element => element.Id == elementId)
            .Select(element => element.ParentId)
            .FirstOrDefaultAsync();

        return diagramId.HasValue
            ? await GetMeetingIdForDiagramAsync(diagramId.Value)
            : null;
    }

    private Task<bool> CanCurrentUserDrawDiagramAsync(Guid diagramId)
    {
        return db.MeetingParticipants.AnyAsync(participant =>
            participant.Meeting.BoardId == diagramId &&
            participant.Meeting.IsActive &&
            participant.TeamMember.UserId == currentUser.Id &&
            participant.CanDraw);
    }

    private async Task<bool> CanCurrentUserDrawElementAsync(Guid elementId)
    {
        var diagramId = await db.DiagramElements
            .Where(element => element.Id == elementId)
            .Select(element => element.ParentId)
            .FirstOrDefaultAsync();

        return diagramId.HasValue && await CanCurrentUserDrawDiagramAsync(diagramId.Value);
    }

    private Task<bool> CanCurrentUserViewDiagramAsync(Guid diagramId)
    {
        return db.MeetingParticipants.AnyAsync(participant =>
            participant.Meeting.BoardId == diagramId &&
            participant.Meeting.IsActive &&
            participant.TeamMember.UserId == currentUser.Id);
    }
}

public record MoveDto(int Dx, int Dy);
public record ResizeDto(int Width, int Height);
public record EditTextDto(string Content, int FontSize, string Color, string? Format);
public record AddClassBoxDto(
    int X1,
    int Y1,
    int X2,
    int Y2,
    IReadOnlyCollection<string>? Attributes,
    IReadOnlyCollection<string>? Methods);
public record AddLineDto(
    Guid StartBoxId,
    Guid EndBoxId,
    string? MiddleText,
    string? Text1,
    string? Text2);
