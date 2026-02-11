using GalaxyUML.Data.Mappers;
using Microsoft.EntityFrameworkCore;
using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Data.Repositories.Implementations
{
    public class TeamRepo : ITeamRepo
    {
        private readonly AppDbContext _context;
        
        public TeamRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Team team)
        {
            // if (await _context.Teams.AnyAsync(t => t.Id == team.IdTeam))
            //     throw new Exception("Team with this id already exists.");
            // if (await _context.Teams.AnyAsync(t => t.TeamCode == team.TeamCode))
            //     throw new Exception("Team with this code already exists.");

            var entity = TeamMapper.ToEntity(team);
            _context.Teams.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
                throw new Exception("Team with this id doesn't exist.");

            _context.Teams.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams
                            .AsNoTracking()
                            .Select(t => TeamMapper.ToModel(t))
                            .ToListAsync();
        }

        public async Task<Team?> GetByCodeAsync(string code)
        {
            var entity = await _context.Teams.FirstOrDefaultAsync(t => t.TeamCode == code);
            return entity == null ? null : TeamMapper.ToModel(entity);
        }

        public async Task<Team?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            return entity == null ? null : TeamMapper.ToModel(entity);
        }

        public async Task<IEnumerable<Team>> GetByOwnerAsync(Guid idOwner)
        {
            return await _context.Teams
                   .AsNoTracking()
                   .Where(t => t.IdTeamOwner == idOwner)
                   .Select(t => TeamMapper.ToModel(t))
                   .ToListAsync();
        }

        public async Task UpdateAsync(Guid id, Team team)
        {
            var entity = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
                throw new Exception("Team with this id doesn't exist.");

            _context.Teams.Update(TeamMapper.ToEntity(team));
            await _context.SaveChangesAsync();
        }
    }
}