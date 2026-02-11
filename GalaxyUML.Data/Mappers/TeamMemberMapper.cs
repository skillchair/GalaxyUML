using TeamMember = GalaxyUML.Core.Models.TeamMember;
using TeamMemberEntity = GalaxyUML.Data.Entities.TeamMemberEntity;

namespace GalaxyUML.Data.Mappers
{
    static class TeamMemberMapper
    {
        public static TeamMember ToModel(TeamMemberEntity entity)
        {
            return new TeamMember(
                entity.IdTeam,
                TeamMapper.ToModel(entity.Team),
                UserMapper.ToModel(entity.Member),
                entity.Role
            );
        }

        public static TeamMemberEntity ToEntity(TeamMember model)
        {
            return new TeamMemberEntity
            {
                //Id = model.IdTeamMember,
                IdTeam = model.IdTeam,
                Team = TeamMapper.ToEntity(model.Team),
                IdMember = model.Member.IdUser,
                Member = UserMapper.ToEntity(model.Member)
            };
        }
    }
}