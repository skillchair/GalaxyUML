using BannedUser = GalaxyUML.Core.Models.BannedUser;
using BannedUserEntity = GalaxyUML.Data.Entities.BannedUserEntity;

namespace GalaxyUML.Data.Mappers
{
    static class BannedUserMapper
    {
        public static BannedUser ToModel(BannedUserEntity entity)
        {
            return new BannedUser(
                entity.IdTeam,
                entity.IdUser,
                TeamMapper.ToModel(entity.Team),
                UserMapper.ToModel(entity.User)
            );
        }

        public static BannedUserEntity ToEntity(BannedUser model)
        {
            return new BannedUserEntity
            {
                //Id = model.IdBan,
                IdUser = model.User.IdUser,
                User = UserMapper.ToEntity(model.User),
                IdTeam = model.IdTeam,
                Team = TeamMapper.ToEntity(model.Team)
            };
        }
    }
}