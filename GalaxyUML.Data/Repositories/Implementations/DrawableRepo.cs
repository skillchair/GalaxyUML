using IDrawable = GalaxyUML.Core.Models.IDrawable;
using Diagram = GalaxyUML.Core.Models.Diagram;
using Team = GalaxyUML.Core.Models.Team;
using DrawableMapper = GalaxyUML.Data.Mappers.DrawableMapper;
using ObjectType = GalaxyUML.Core.Models.ObjectType;
using Microsoft.EntityFrameworkCore;

namespace GalaxyUML.Data.Repositories.Implementations
{
    class DrawableRepo : IDrawableRepo
    {
        AppDbContext _context;

        public DrawableRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(IDrawable drawable/*, Diagram parent, Team team*/)
        {
            // var teamEntity = await _context.Teams.FindAsync(team);
            // if (teamEntity == null)
            //     throw new Exception("Drawable with this id already exists.");
            
            // var parentEntity = await _context.Diagrams.FindAsync(parent);
            // if (parentEntity == null)
            //     throw new Exception("Parent object not found.");

            var entity = DrawableMapper.ToEntity(drawable);//, parentEntity/*, teamEntity*/);
            _context.Drawables.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Drawables.FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
                throw new Exception("Drawable object with this id doesn't exist.");

            _context.Drawables.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<IDrawable>> GetAllAsync()
        {
            return await _context.Drawables
                            .AsNoTracking()
                            .Select(d => DrawableMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task<IDrawable?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Drawables.FirstOrDefaultAsync(d => d.Id == id);
            return entity == null ? null : DrawableMapper.ToModel(entity);
        }

        public async Task<IEnumerable<IDrawable>> GetByParentAsync(Guid idParent)
        {
            return await _context.Drawables
                            .AsNoTracking()
                            .Where(d => d.IdParent == idParent)
                            .Select(d => DrawableMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task<IEnumerable<IDrawable>> GetByParentTypeAsync(Guid idParent, ObjectType type)
        {
            return await _context.Drawables
                            .AsNoTracking()
                            .Where(d => d.IdParent == idParent && d.Type == type)
                            .Select(d => DrawableMapper.ToModel(d))
                            .ToListAsync();
        }

        public async Task UpdateAsync(Guid id, IDrawable drawable)
        {
            var entity = await _context.Drawables.FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
                throw new Exception("Drawable object with this id doesn't exist.");

            _context.Drawables.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}