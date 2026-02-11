using IDrawable = GalaxyUML.Core.Models.IDrawable;
using Diagram = GalaxyUML.Core.Models.Diagram;
using Team = GalaxyUML.Core.Models.Team;
using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Repositories
{
    interface IDrawableRepo
    {
        Task<IDrawable?> GetByIdAsync(Guid id);
        Task<IEnumerable<IDrawable>> GetByParentAsync(Guid idParent);
        Task<IEnumerable<IDrawable>> GetByParentTypeAsync(Guid idParent, ObjectType type, Guid idMeeting);  // namerno i idMeeting iz sigurnosnih razloga
        Task<IEnumerable<IDrawable>> GetAllAsync();

        Task CreateAsync(IDrawable drawable, Diagram parent, Team team);
        Task UpdateAsync(IDrawable drawable);
        Task DeleteAsync(Guid id);
    }
}