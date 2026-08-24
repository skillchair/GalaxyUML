using GalaxyUML.Core.Models;
using GalaxyUML.Data.Repositories;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Core.Services;

public class UserService
{
    private readonly IUserRepo _users;
    public UserService(IUserRepo users) => _users = users;

    public async Task<Guid> RegisterAsync(string first, string last, string username, string email, string password)
    {
        if (await _users.GetByUsernameAsync(username) is not null)
            throw new InvalidOperationException("Username is already taken");

        if (await _users.GetByEmailAsync(email) is not null)
            throw new InvalidOperationException("Email is already registered");

        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(Guid.NewGuid(), first, last, username, email, hash);

        try
        {
            await _users.AddAsync(user);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Username or email is already in use");
        }

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
