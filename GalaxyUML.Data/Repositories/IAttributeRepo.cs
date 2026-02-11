using Attribute = GalaxyUML.Core.Models.Attribute;

namespace GalaxyUML.Data.Repositories
{
    public interface IAttributeRepo
    {
        Task<Attribute?> GetByIdAsync(Guid id);
        Task<IEnumerable<Attribute>> GetByClassBoxAsync(Guid idClassBox);
        Task CreateAsync(Attribute attribute);
        Task UpdateAsync(Guid id, Attribute attribute);
        Task DeleteAsync(Guid id);
    }
}