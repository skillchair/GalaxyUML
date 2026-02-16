using System.Reflection;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;
using GalaxyUML.Data;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Core.Services;

public class MeetingService
{
    private readonly IMeetingRepo _meetings;
    private readonly AppDbContext _db;

    public MeetingService(IMeetingRepo meetings, AppDbContext db)
    {
        _meetings = meetings;
        _db = db;
    }

    public async Task<MeetingStartedDto> CreateAsync(Guid teamId, Guid organizerId)
    {
        var team = await _db.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId)
            ?? throw new InvalidOperationException("Team not found");

        if (team.OwnerId != organizerId)
            throw new InvalidOperationException("Only team owner can start meeting");

        if (team.CurrentMeetingId is not null)
            throw new InvalidOperationException("Meeting already active");

        if (!team.Members.Any(m => m.UserId == organizerId))
            throw new InvalidOperationException("Organizer is not a team member");

        var meetingId = Guid.NewGuid();
        team.CurrentMeetingId = meetingId;

        var meeting = Meeting.Create(teamId, organizerId);
        // force same Id as reserved in team
        typeof(Meeting).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                       .SetValue(meeting, meetingId);

        await _meetings.AddAsync(meeting);
        return new MeetingStartedDto(meetingId, meeting.Board.Id, teamId, DateTime.UtcNow);
    }

    public async Task JoinAsync(Guid meetingId, Guid userId)
    {
        var meeting = await _meetings.GetByIdAsync(meetingId) ?? throw new InvalidOperationException("Meeting not found");
        meeting.Join(userId);
        await _meetings.SaveAsync();
    }

    public async Task LeaveAsync(Guid meetingId, Guid userId)
    {
        var meeting = await _meetings.GetByIdAsync(meetingId) ?? throw new InvalidOperationException("Meeting not found");
        meeting.Leave(userId);
        await _meetings.SaveAsync();
    }

    public async Task GrantDrawAsync(Guid meetingId, Guid actorId, Guid targetId, bool canDraw)
    {
        var meeting = await _meetings.GetByIdAsync(meetingId) ?? throw new InvalidOperationException("Meeting not found");
        meeting.GrantDraw(actorId, targetId, canDraw);
        await _meetings.SaveAsync();
    }

    public async Task AddMessageAsync(Guid meetingId, Guid senderId, string content)
    {
        var meeting = await _meetings.GetByIdAsync(meetingId) ?? throw new InvalidOperationException("Meeting not found");
        meeting.AddMessage(senderId, content);
        await _meetings.SaveAsync();
    }

    public async Task DeleteAsync(Guid meetingId)
    {
        var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == meetingId);
        if (meeting is not null)
        {
            var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == meeting.TeamId);
            if (team?.CurrentMeetingId == meetingId)
            {
                team.CurrentMeetingId = null;
            }
        }

        await _meetings.RemoveAsync(meetingId);
    }

    public async Task EndAsync(Guid meetingId, Guid actorUserId)
    {
        var meeting = await _db.Meetings.FirstOrDefaultAsync(m => m.Id == meetingId)
            ?? throw new InvalidOperationException("Meeting not found");

        var team = await _db.Teams.FirstOrDefaultAsync(t => t.Id == meeting.TeamId)
            ?? throw new InvalidOperationException("Team not found");

        if (team.OwnerId != actorUserId)
            throw new InvalidOperationException("Only team owner can end meeting");

        if (!meeting.IsActive)
            return;

        meeting.IsActive = false;
        meeting.EndingTime = DateTime.UtcNow;

        if (team.CurrentMeetingId == meetingId)
        {
            team.CurrentMeetingId = null;
        }

        await _db.SaveChangesAsync();
    }
}

public record MeetingStartedDto(Guid MeetingId, Guid BoardId, Guid TeamId, DateTime StartedAtUtc);
