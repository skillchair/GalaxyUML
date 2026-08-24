using GalaxyUML.Core.Models;

namespace GalaxyUML.Data.Repositories;

public interface IUserRepo
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveAsync();
}
