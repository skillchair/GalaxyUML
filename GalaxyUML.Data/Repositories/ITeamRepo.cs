using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Data.Repositories
{
    public interface ITeamRepo
    {
        Task<Team?> GetByIdAsync(Guid id);
        Task<Team?> GetByCodeAsync(string code);
        Task<IEnumerable<Team>> GetAllByOwnerAsync(Guid idOwner);
        Task<IEnumerable<Team>> GetAllAsync();

        Task CreateAsync(Team team);
        Task UpdateAsync(Team team);
        Task DeleteAsync(Team team);
    }
}