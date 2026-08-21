// controls authenticated membership in diagram realtime groups.

using GalaxyUML.Api.Security;
using GalaxyUML.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Api.Hubs;

[Authorize]
public class DiagramHub(AppDbContext db, ICurrentUser currentUser) : Hub
{
    public async Task JoinMeeting(Guid meetingId)
    {
        var userId = currentUser.GetId(Context.User);
        var isParticipant = await db.MeetingParticipants.AnyAsync(participant =>
            participant.MeetingId == meetingId &&
            participant.Meeting.IsActive &&
            participant.TeamMember.UserId == userId);

        if (!isParticipant)
        {
            throw new HubException("Only active meeting participants can join this group.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, meetingId.ToString());
    }

    public Task LeaveMeeting(Guid meetingId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, meetingId.ToString());
    }
}
