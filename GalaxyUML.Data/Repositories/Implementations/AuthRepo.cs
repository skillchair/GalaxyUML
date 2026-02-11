using GalaxyUML.Core.Models;
using GalaxyUML.Core.Security;

namespace GalaxyUML.Data.Repositories.Implementations
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IUserRepo _userRepo;

        public AuthRepo(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var entity = await _userRepo.GetByUsernameAsync(username);
            if (entity == null)
                return null;

            if (PasswordHelper.VerifyPassword(password, entity.Password))
                return entity;

            return null;
        }
    }
}