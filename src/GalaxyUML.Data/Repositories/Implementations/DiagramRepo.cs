using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;
using GalaxyUML.Data.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories.Implementations;

public class DiagramRepo : IDiagramRepo
{
    private readonly AppDbContext _db;
    public DiagramRepo(AppDbContext db) => _db = db;

    public async Task<IDiagram?> GetByIdAsync(Guid id)
    {
        var e = await _db.Diagrams
            .Include(d => d.Children)
            .FirstOrDefaultAsync(d => d.Id == id);
        return e == null ? null : DiagramMapper.ToDomain(e, new Diagram());
    }

    public async Task AddAsync(IDiagram diagram)
    {
        var entity = DiagramMapper.ToEntity(diagram); // returns DiagramElementEntity
        _db.Set<DiagramElementEntity>().Add(entity);  // or cast to DiagramEntity if you only add roots
        await _db.SaveChangesAsync();
    }


    public async Task RemoveAsync(Guid id)
    {
        var entity = await _db.Diagrams.FindAsync(id) ?? throw new InvalidOperationException("Diagram not found");
        _db.Diagrams.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
