using TeamMember = GalaxyUML.Core.Models.TeamMember;
using TeamMemberMapper = GalaxyUML.Data.Mappers.TeamMemberMapper;
using RoleEnum = GalaxyUML.Core.Models.RoleEnum;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories
{
    class TeamMemberRepo : ITeamMemberRepo
    {
        AppDbContext _context;

        public TeamMemberRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(TeamMember teamMember)
        {
            if (_context.Members.Any(tm => tm.Id == teamMember.IdTeamMember))
                throw new Exception("TeamMember with this id already exists.");

            var entity = TeamMemberMapper.ToEntity(teamMember);
            await _context.Members.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TeamMember teamMember)
        {
            var entity = await _context.Members.FirstOrDefaultAsync(tm => tm.Id == teamMember.IdTeamMember);
            if (entity == null)
                throw new Exception("TeamMember with this id doesn't exist.");

            _context.Members.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<TeamMember?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TeamMember>> GetByTeamAsync(Guid idTeam)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TeamMember>> GetByTeamRoleAsync(Guid idTeam, RoleEnum role)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TeamMember>> GetByUserAsync(Guid idUser)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TeamMember>> GetByUserRoleAsync(Guid idUser, RoleEnum role)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TeamMember teamMember)
        {
            throw new NotImplementedException();
        }
    }
}