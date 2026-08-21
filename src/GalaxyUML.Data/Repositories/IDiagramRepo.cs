using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Repositories;

public interface IDiagramRepo
{
    Task<IDiagram?> GetByIdAsync(Guid id);
    Task AddAsync(IDiagram diagram);
    Task RemoveAsync(Guid id);
    Task SaveAsync();
}
