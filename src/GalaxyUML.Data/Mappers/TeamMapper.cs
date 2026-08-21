using System.Reflection;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class TeamMapper
{
    public static Team ToDomain(TeamEntity e)
    {
        var team = new Team(e.Id, e.OwnerId, e.TeamName, e.TeamCode);

        foreach (var m in e.Members)
            team.Join(m.UserId, e.TeamCode);

        foreach (var m in e.Members.Where(x => x.Role != RoleEnum.Member))
            team.ChangeRole(e.OwnerId, m.UserId, m.Role);

        foreach (var b in e.BannedUsers)
            team.Ban(e.OwnerId, b.UserId, b.Reason);

        if (e.CurrentMeetingId is Guid mid)
            team.StartMeeting(mid, e.OwnerId);

        return team;
    }

    public static TeamEntity ToEntity(Team d)
    {
        var e = new TeamEntity
        {
            Id = d.Id,
            OwnerId = d.OwnerId,
            TeamName = d.TeamName,
            TeamCode = d.TeamCode,
            CurrentMeetingId = d.CurrentMeetingId
        };

        e.Members = d.Members.Select(m => TeamMemberMapper.ToEntity(d.Id, m)).ToList();
        e.BannedUsers = d.BannedUsers.Select(b => BannedUserMapper.ToEntity(d.Id, b)).ToList();
        return e;
    }
}
