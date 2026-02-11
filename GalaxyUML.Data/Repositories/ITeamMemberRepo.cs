using TeamMember = GalaxyUML.Core.Models.TeamMember;
using RoleEnum = GalaxyUML.Core.Models.RoleEnum;

namespace GalaxyUML.Data.Repositories
{
    public interface ITeamMemberRepo
    {
        Task<TeamMember?> GetByIdAsync(Guid id);
        Task<IEnumerable<TeamMember>> GetByTeamAsync(Guid idTeam);
        Task<IEnumerable<TeamMember>> GetByTeamRoleAsync(Guid idTeam, RoleEnum role);
        Task<IEnumerable<TeamMember>> GetByUserAsync(Guid idUser);
        Task<IEnumerable<TeamMember>> GetByUserRoleAsync(Guid idUser, RoleEnum role);
        Task<IEnumerable<TeamMember>> GetAllAsync();

        Task CreateAsync(TeamMember teamMember);
        Task UpdateAsync(TeamMember teamMember);
        Task DeleteAsync(Guid id);
    }
}