using GalaxyUML.Data.Entities;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Mappers;

namespace GalaxyUML.Data.Repositories
{
    class UserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public UserEntity? GetByUsername(string username)
        {
            return _context.Set<UserEntity>()
                           .FirstOrDefault(u => u.Username == username);
        }

        public void Register(User user)
        {
            // Provera da li username ili email već postoje
            if (_context.Set<UserEntity>()
                        .Any(u => u.Username == user.Username || u.Email == user.Email))
            {
                throw new Exception("Username or email already exists.");
            }

            var entity = UserMapper.ToEntity(user);
            _context.Set<UserEntity>().Add(entity);
            _context.SaveChanges();
        }
    }
}