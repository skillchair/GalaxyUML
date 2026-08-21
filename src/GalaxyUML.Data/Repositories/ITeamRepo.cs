using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Repositories;

public interface ITeamRepo
{
    Task<Team?> GetByIdAsync(Guid id);
    Task AddAsync(Team team);
    Task RemoveAsync(Guid id);
    Task SaveAsync();
}
