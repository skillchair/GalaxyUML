using GalaxyUML.Data.Entities;
using Team = GalaxyUML.Core.Models.Team;
using TeamEntity = GalaxyUML.Data.Entities.TeamEntity;

namespace GalaxyUML.Data.Mappers
{
    static class TeamMapper
    {
        public static Team ToModel(TeamEntity entity)
        {
            return new Team(
                entity.Id,
                entity.IdTeamOwner,
                entity.TeamName,
                UserMapper.ToModel(entity.TeamOwner.Member)
            );
        }

        public static TeamEntity ToEntity(Team team)
        {
            TeamEntity teamEntity = new TeamEntity
            {
                //Id = team.IdTeam,
                TeamName = team.TeamName,
                IdTeamOwner = team.IdOwner,
                TeamOwner = TeamMemberMapper.ToEntity(team.TeamOwner),
                TeamCode = team.TeamCode,
                //Meetings = new List<MeetingEntity>(),   // prazno za sad
                //Members = new List<TeamMemberEntity>(),
                //BannedUsers = new List<BannedUserEntity>()
            };

            //List<TeamMemberEntity> memberEntities = new List<TeamMemberEntity>();
            //foreach (var m in team.Members)
                //memberEntities.Add(TeamMemberMapper.ToEntity(m));

            // teamEntity.Members = memberEntities;

            // List<MeetingEntity> meetingEntities = new List<MeetingEntity>();
            // foreach (var m in team.Meetings)
            //     meetingEntities.Add(MeetingMapper.ToEntity(m/*, teamEntity*/));

            // teamEntity.Meetings = meetingEntities;

            // List<BannedUserEntity> bannedUserEntities = new List<BannedUserEntity>();
            // foreach (var b in team.BannedUsers)
            //     bannedUserEntities.Add(BannedUserMapper.ToEntity(b));

            // teamEntity.BannedUsers = bannedUserEntities;

            return teamEntity;
        }
    }
}