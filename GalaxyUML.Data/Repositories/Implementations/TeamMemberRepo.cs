using TeamMember = GalaxyUML.Core.Models.TeamMember;
using TeamMemberMapper = GalaxyUML.Data.Mappers.TeamMemberMapper;
using RoleEnum = GalaxyUML.Core.Models.RoleEnum;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories.Implementations
{
    public class TeamMemberRepo : ITeamMemberRepo
    {
        AppDbContext _context;

        public TeamMemberRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(TeamMember teamMember)
        {
            // if (await _context.Members.AnyAsync(tm => tm.Id == teamMember.IdTeamMember))
            //     throw new Exception("TeamMember with this id already exists.");

            var entity = TeamMemberMapper.ToEntity(teamMember);
            _context.Members.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Members.FirstOrDefaultAsync(tm => tm.Id == id);
            if (entity == null)
                throw new Exception("TeamMember with this id doesn't exist.");

            _context.Members.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TeamMember>> GetAllAsync()
        {
            return await _context.Members
                        .AsNoTracking()
                        .Select(m => TeamMemberMapper.ToModel(m))
                        .ToListAsync();
        }

        public async Task<TeamMember?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
            return entity == null ? null : TeamMemberMapper.ToModel(entity);
        }

        public async Task<IEnumerable<TeamMember>> GetByTeamAsync(Guid idTeam)
        {
            return await _context.Members
                        .AsNoTracking()
                        .Where(m => m.IdTeam == idTeam)
                        .Select(m => TeamMemberMapper.ToModel(m))
                        .ToListAsync();
        }

        public async Task<IEnumerable<TeamMember>> GetByTeamRoleAsync(Guid idTeam, RoleEnum role)
        {
            return await _context.Members
                        .AsNoTracking()
                        .Where(m => m.IdTeam == idTeam && m.Role == role)
                        .Select(m => TeamMemberMapper.ToModel(m))
                        .ToListAsync();
        }

        public async Task<IEnumerable<TeamMember>> GetByUserAsync(Guid idUser)
        {
            return await _context.Members
                        .AsNoTracking()
                        .Where(m => m.IdMember == idUser)
                        .Select(m => TeamMemberMapper.ToModel(m))
                        .ToListAsync();
        }

        public async Task<IEnumerable<TeamMember>> GetByUserRoleAsync(Guid idUser, RoleEnum role)
        {
            return await _context.Members
                        .AsNoTracking()
                        .Where(m => m.IdMember == idUser && m.Role == role)
                        .Select(m => TeamMemberMapper.ToModel(m))
                        .ToListAsync();
        }

        public async Task UpdateAsync(Guid id, TeamMember teamMember)
        {
            var entity = await _context.Members.FirstOrDefaultAsync(tm => tm.Id == id);
            if (entity == null)
                throw new Exception("TeamMember with this id doesn't exist.");

            _context.Members.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}