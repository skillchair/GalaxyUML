using GalaxyUML.Core.Models;
using GalaxyUML.Core.Security;
using GalaxyUML.Data.Mappers;

namespace GalaxyUML.Data.Repositories
{
    class AuthRepo
    {
        private readonly UserRepo _userRepo;

        public AuthRepo(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public User? Login(string username, string password)
        {
            var entity = _userRepo.GetByUsername(username);
            if (entity == null)
                return null;

            if (PasswordHelper.VerifyPassword(password, entity.Password))
                return UserMapper.ToModel(entity);

            return null;
        }
    }
}