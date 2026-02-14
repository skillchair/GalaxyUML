using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Repositories;

public interface IUserRepo
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task SaveAsync();
}
