using Microsoft.EntityFrameworkCore;
using BannedUser = GalaxyUML.Core.Models.BannedUser;
using BannedUserMapper = GalaxyUML.Data.Mappers.BannedUserMapper;

namespace GalaxyUML.Data.Repositories
{
    class BannedUserRepo : IBannedUserRepo
    {
        AppDbContext _context;

        public BannedUserRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(BannedUser bannedUser)
        {
            if (await _context.BannedUsers.AnyAsync(b => b.Id == bannedUser.IdBan))
                throw new Exception("User with this id doesn't exist.");

            var entity = BannedUserMapper.ToEntity(bannedUser);
            _context.BannedUsers.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BannedUser bannedUser)
        {
            var entity = await _context.BannedUsers.FirstOrDefaultAsync(b => b.Id == bannedUser.IdBan);
            if (entity == null)
                throw new Exception("User with this id doesn't exist.");
            
            _context.BannedUsers.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BannedUser>> GetAllAsync()
        {
            return await _context.BannedUsers
                            .AsNoTracking()
                            .Select(b => BannedUserMapper.ToModel(b))
                            .ToListAsync();
        }

        public async Task<BannedUser?> GetByIdAsync(Guid id)
        {
            var entity = await _context.BannedUsers.FirstOrDefaultAsync(b => b.Id == id);
            return entity == null ? null : BannedUserMapper.ToModel(entity);
        }

        public async Task<IEnumerable<BannedUser>> GetByTeamAsync(Guid idTeam)
        {
            return await _context.BannedUsers
                            .AsNoTracking()
                            .Where(b => b.IdTeam == idTeam)
                            .Select(b => BannedUserMapper.ToModel(b))
                            .ToListAsync();
        }

        public async Task<IEnumerable<BannedUser>> GetByUserAsync(Guid idUser)
        {
            return await _context.BannedUsers
                            .AsNoTracking()
                            .Where(b => b.IdUser == idUser)
                            .Select(b => BannedUserMapper.ToModel(b))
                            .ToListAsync();
        }

        public async Task UpdateAsync(BannedUser bannedUser)
        {
            var entity = await _context.BannedUsers.FirstOrDefaultAsync(b => b.Id == bannedUser.IdBan);
            if (entity == null)
                throw new Exception("User with this id doesn't exist.");
            
            _context.BannedUsers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}