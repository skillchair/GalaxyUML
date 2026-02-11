using Method = GalaxyUML.Core.Models.Method;

namespace GalaxyUML.Data.Repositories
{
    public interface IMethodRepo
    {
        Task<Method?> GetByIdAsync(Guid id);
        Task<IEnumerable<Method>> GetByClassBoxAsync(Guid idClassBox);
        Task<IEnumerable<Method>> GetAllAsync();

        Task CreateAsync(Method method);
        Task UpdateAsync(Guid id, Method method);
        Task DeleteAsync(Guid id);
    }
}