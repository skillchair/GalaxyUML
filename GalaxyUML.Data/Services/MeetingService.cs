using System.Reflection;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;

namespace GalaxyUML.Core.Services;

public class MeetingService
{
    private readonly IMeetingRepo _meetings;
    private readonly ITeamRepo _teams;

    public MeetingService(IMeetingRepo meetings, ITeamRepo teams)
    {
        _meetings = meetings;
        _teams = teams;
    }

    public async Task<Guid> CreateAsync(Guid teamId, Guid organizerId)
    {
        var team = await _teams.GetByIdAsync(teamId) ?? throw new InvalidOperationException("Team not found");

        var meetingId = Guid.NewGuid();
        team.StartMeeting(meetingId, organizerId);

        var meeting = Meeting.Create(teamId, organizerId);
        // force same Id as reserved in team
        typeof(Meeting).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                       .SetValue(meeting, meetingId);

        await _meetings.AddAsync(meeting);
        await _teams.SaveAsync();
        return meetingId;
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
        await _meetings.RemoveAsync(meetingId);
    }
}
