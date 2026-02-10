using Microsoft.EntityFrameworkCore;
using User = GalaxyUML.Core.Models.User;
using UserMapper = GalaxyUML.Data.Mappers.UserMapper;

namespace GalaxyUML.Data.Repositories
{
    class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Id == user.IdUser))
                throw new Exception("User with this id already exists.");
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                throw new Exception("User with this username already exists.");
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                throw new Exception("User with this email already exists.");

            var entity = UserMapper.ToEntity(user);
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (entity == null)
                throw new Exception("Invalid user id.");

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                            .AsNoTracking()
                            .Select(u => UserMapper.ToModel(u))
                            .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return entity == null ? null : UserMapper.ToModel(entity);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return entity == null ? null : UserMapper.ToModel(entity);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return entity == null ? null : UserMapper.ToModel(entity);
        }

        public async Task UpdateAsync(User user)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.IdUser);
            if (entity == null)
                throw new Exception("User doesn't exist.");

            _context.Users.Update(UserMapper.ToEntity(user)); 
            await _context.SaveChangesAsync();
        }
    }
}