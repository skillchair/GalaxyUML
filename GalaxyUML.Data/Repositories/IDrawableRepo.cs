using IDrawable = GalaxyUML.Core.Models.IDrawable;
using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Repositories
{
    public interface IDrawableRepo
    {
        Task<IDrawable?> GetByIdAsync(Guid id);
        Task<IEnumerable<IDrawable>> GetByParentAsync(Guid idParent);
        Task<IEnumerable<IDrawable>> GetByParentTypeAsync(Guid idParent, ObjectType type);  // namerno i idMeeting iz sigurnosnih razloga
        Task<IEnumerable<IDrawable>> GetAllAsync();

        Task CreateAsync(IDrawable drawable/*, Diagram parent, Team team*/);
        Task UpdateAsync(Guid id, IDrawable drawable);
        Task DeleteAsync(Guid id);
    }
}