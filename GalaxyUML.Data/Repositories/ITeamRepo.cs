using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Data.Repositories
{
    public interface ITeamRepo
    {
        Task<Team?> GetByIdAsync(Guid id);
        Task<Team?> GetByCodeAsync(string code);
        Task<IEnumerable<Team>> GetByOwnerAsync(Guid idOwner);
        Task<IEnumerable<Team>> GetAllAsync();

        Task CreateAsync(Team team);
        Task UpdateAsync(Guid id, Team team);
        Task DeleteAsync(Guid id);
    }
}