using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;
using GalaxyUML.Data.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories.Implementations;

public class MeetingRepo : IMeetingRepo
{
    private readonly AppDbContext _db;
    public MeetingRepo(AppDbContext db) => _db = db;

    public async Task<Meeting?> GetByIdAsync(Guid id)
    {
        var e = await _db.Meetings
            .Include(m => m.Board).ThenInclude(d => d.Children)
            .Include(m => m.Chat).ThenInclude(c => c.Messages)
            .Include(m => m.Participants).ThenInclude(p => p.TeamMember)
            .FirstOrDefaultAsync(m => m.Id == id);

        return e == null ? null : MeetingMapper.ToDomain(e);
    }

    public async Task AddAsync(Meeting meeting)
    {
        _db.Meetings.Add(MeetingMapper.ToEntity(meeting));
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _db.Meetings.FindAsync(id) ?? throw new InvalidOperationException("Meeting not found");
        _db.Meetings.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
