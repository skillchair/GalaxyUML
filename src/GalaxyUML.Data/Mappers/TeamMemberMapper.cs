using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class TeamMemberMapper
{
    public static TeamMember ToDomain(TeamMemberEntity e) =>
        new TeamMember(e.UserId, e.Role);

    public static TeamMemberEntity ToEntity(Guid teamId, TeamMember d) => new()
    {
        Id = Guid.NewGuid(),
        TeamId = teamId,
        UserId = d.UserId,
        Role = d.Role,
        JoinedAt = d.JoinedAt
    };
}
