using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;
using GalaxyUML.Data.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories.Implementations;

public class TeamRepo : ITeamRepo
{
    private readonly AppDbContext _db;
    public TeamRepo(AppDbContext db) => _db = db;

    public async Task<Team?> GetByIdAsync(Guid id) =>
        (await _db.Teams
            .Include(t => t.Members)
            .Include(t => t.BannedUsers)
            .FirstOrDefaultAsync(t => t.Id == id))
        is TeamEntity e ? TeamMapper.ToDomain(e) : null;

    public async Task AddAsync(Team team)
    {
        _db.Teams.Add(TeamMapper.ToEntity(team));
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _db.Teams.FindAsync(id) ?? throw new InvalidOperationException("Team not found");
        _db.Teams.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
