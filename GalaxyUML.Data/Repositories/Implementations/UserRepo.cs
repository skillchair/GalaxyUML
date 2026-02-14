using GalaxyUML.Core.Models;
using GalaxyUML.Data.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories.Implementations;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _db;
    public UserRepo(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id) =>
        (await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id))
        is var e && e != null ? UserMapper.ToDomain(e) : null;

    public async Task<User?> GetByUsernameAsync(string username) =>
        (await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username))
        is var e && e != null ? UserMapper.ToDomain(e) : null;

    public async Task AddAsync(User user)
    {
        _db.Users.Add(UserMapper.ToEntity(user));
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
