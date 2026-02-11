using Microsoft.EntityFrameworkCore;
using Method = GalaxyUML.Core.Models.Method;
using MethodMapper = GalaxyUML.Data.Mappers.MethodMapper;

namespace GalaxyUML.Data.Repositories.Implementations
{
    public class MethodRepo : IMethodRepo
    {
        AppDbContext _context;

        public MethodRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Method method)
        {
            var entity = MethodMapper.ToEntity(method);
            _context.Methods.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Methods.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
                throw new Exception("Method with this id doesn't exist.");
            
            _context.Methods.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Method>> GetAllAsync()
        {
            return await _context.Methods
                            .AsNoTracking()
                            .Select(m => MethodMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Method>> GetByClassBoxAsync(Guid idClassBox)
        {
            return await _context.Methods
                            .AsNoTracking()
                            .Where(m => m.IdClassBox == idClassBox)
                            .Select(m => MethodMapper.ToModel(m))
                            .ToListAsync();
        }

        public async Task<Method?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Methods.FirstOrDefaultAsync(m => m.Id == id);
            return entity == null ? null : MethodMapper.ToModel(entity);
        }

        public async Task UpdateAsync(Guid id, Method method)
        {
            var entity = await _context.Methods.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
                throw new Exception("Method with this id doesn't exist.");

            _context.Methods.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}