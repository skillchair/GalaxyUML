using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;
using BCrypt.Net;

namespace GalaxyUML.Core.Services;

public class UserService
{
    private readonly IUserRepo _users;
    public UserService(IUserRepo users) => _users = users;

    public async Task<Guid> RegisterAsync(string first, string last, string username, string email, string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(Guid.NewGuid(), first, last, username, email, hash);
        await _users.AddAsync(user);
        return user.IdUser;
    }

    public Task<User?> GetAsync(Guid id) => _users.GetByIdAsync(id);

    public async Task<User?> ValidateAsync(string username, string password)
    {
        var user = await _users.GetByUsernameAsync(username);
        if (user is null) return null;
        return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
    }
}
