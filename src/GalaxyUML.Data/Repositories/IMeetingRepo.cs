using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Repositories;

public interface IMeetingRepo
{
    Task<Meeting?> GetByIdAsync(Guid id);
    Task AddAsync(Meeting meeting);
    Task RemoveAsync(Guid id);
    Task SaveAsync();
}
