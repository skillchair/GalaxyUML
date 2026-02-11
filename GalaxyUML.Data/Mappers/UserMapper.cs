using User = GalaxyUML.Core.Models.User;
using UserEntity = GalaxyUML.Data.Entities.UserEntity;

namespace GalaxyUML.Data.Mappers
{
    static class UserMapper
    {
        public static User ToModel(UserEntity entity)
        {
            return new User(
                entity.Id,
                entity.FirstName,
                entity.LastName,
                entity.Username,
                entity.Email,
                entity.Password // vec hash
            );
        }

        public static UserEntity ToEntity(User model)
        {
            return new UserEntity
            {
                Id = model.IdUser,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                Password = model.Password // hash
            };
        }
    }

}