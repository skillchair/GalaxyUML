using Microsoft.EntityFrameworkCore;
using Attribute = GalaxyUML.Core.Models.Attribute;
using AttributeMapper = GalaxyUML.Data.Mappers.AttributeMapper;

namespace GalaxyUML.Data.Repositories.Implementations
{
    class AttributeRepo : IAttributeRepo
    {
        private readonly AppDbContext _context;

        public AttributeRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Attribute attribute)
        {
            var entity = AttributeMapper.ToEntity(attribute);
            _context.Attributes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Attributes.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
                throw new Exception("Attribute with this id doesn't exist.");

            _context.Attributes.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Attribute>> GetAllAsync()
        {
            return await _context.Attributes
                            .AsNoTracking()
                            .Select(a => AttributeMapper.ToModel(a))
                            .ToListAsync();
        }

        public async Task<IEnumerable<Attribute>> GetByClassBoxAsync(Guid idClassBox)
        {
            return await _context.Attributes
                            .AsNoTracking()
                            .Where(a => a.IdClassBox == idClassBox)
                            .Select(a => AttributeMapper.ToModel(a))
                            .ToListAsync();
        }

        public async Task<Attribute?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Attributes.FirstOrDefaultAsync(a => a.Id == id);
            return entity == null ? null : AttributeMapper.ToModel(entity);
        }

        public async Task UpdateAsync(Guid id, Attribute attribute)
        {
            var entity = await _context.Attributes.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
                throw new Exception("Attribute with this id doesn't exist.");

            // Napomena: Ovde bi verovatno trebalo mapirati vrednosti iz 'attribute' u 'entity' 
            // pre pozivanja Update, ali pratim tvoju logiku iz MethodRepo.
            _context.Attributes.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}