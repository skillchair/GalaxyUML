using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class MeetingParticipantMapper
{
    public static MeetingParticipant ToDomain(MeetingParticipantEntity e) =>
        new MeetingParticipant(e.TeamMember.UserId, e.CanDraw);

    public static MeetingParticipantEntity ToEntity(Guid meetingId, Guid teamMemberId, MeetingParticipant d) => new()
    {
        Id = Guid.NewGuid(),
        MeetingId = meetingId,
        TeamMemberId = teamMemberId,
        CanDraw = d.CanDraw,
        JoinedAt = d.JoinedAt
    };
}
