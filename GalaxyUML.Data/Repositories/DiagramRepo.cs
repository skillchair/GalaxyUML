using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Diagram = GalaxyUML.Core.Models.Diagram;
using DiagramMapper = GalaxyUML.Data.Mappers.DiagramMapper;
using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Data.Repositories
{
    class DiagramRepo : IDiagramRepo
    {
        AppDbContext _context;

        public DiagramRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Diagram diagram, Diagram? parent, Team team)
        {
            var teamEntity = await _context.Teams.FindAsync(team.IdTeam);
            if (teamEntity == null) throw new Exception("Team not found.");

            if (await _context.Diagrams.AnyAsync(d => d.Id == diagram.IdDiagram))
                throw new Exception("Diagram with this ID already exists.");

            DiagramEntity? parentEntity = null;
            if (parent != null)
            {
                parentEntity = await _context.Diagrams.FindAsync(parent.IdDiagram);
                if (parentEntity == null) throw new Exception("Parent diagram not found.");
            }

            var entity = DiagramMapper.ToEntity(diagram, parentEntity, teamEntity);

            _context.Diagrams.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Diagram diagram)
        {
            var entity = await _context.Diagrams.FirstOrDefaultAsync(d => d.Id == diagram.IdDiagram);
            if (entity == null)
                throw new Exception("Diagram with this id doesn't exist.");

            _context.Diagrams.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Diagram>> GetAllAsync()
        {
            return await _context.Diagrams
                            .AsNoTracking()
                            .Select(d => DiagramMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task<Diagram?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Diagrams.FirstOrDefaultAsync(d => d.Id == id);
            return entity == null ? null : DiagramMapper.ToModel(entity);
        }

        public async Task<IEnumerable<Diagram>> GetByMeetingAsync(Guid idMeeting)
        {
            return await _context.Diagrams
                            .AsNoTracking()
                            .Where(d => d.IdMeeting == idMeeting)
                            .Select(d => DiagramMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Diagram>> GetByMeetingParentAsync(Guid idMeeting, Guid idParent)
        {
            return await _context.Diagrams
                            .AsNoTracking()
                            .Where(d => d.IdMeeting == idMeeting && d.IdParent == idParent)
                            .Select(d => DiagramMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Diagram>> GetByMeetingTypeAsync(Guid idMeeting, ObjectType type)
        {
            return await _context.Diagrams
                            .AsNoTracking()
                            .Where(d => d.IdMeeting == idMeeting && d.Type == type)
                            .Select(d => DiagramMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task UpdateAsync(Diagram diagram)
        {
            var entity = await _context.Diagrams.FirstOrDefaultAsync(d => d.Id == diagram.IdDiagram);
            if (entity == null)
                throw new Exception("Diagram with this id doesn't exist.");

            _context.Diagrams.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}