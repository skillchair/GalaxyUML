using Diagram = GalaxyUML.Core.Models.Diagram;
using ObjectType = GalaxyUML.Core.Models.ObjectType;

namespace GalaxyUML.Data.Repositories
{
    interface IDiagramRepo
    {
        Task<Diagram?> GetByIdAsync(Guid id);
        Task<IEnumerable<Diagram>> GetByMeetingAsync(Guid idMeeting);
        Task<IEnumerable<Diagram>> GetByMeetingTypeAsync(Guid idMeeting, ObjectType type);
        Task<IEnumerable<Diagram>> GetByMeetingParentAsync(Guid idMeeting, Guid idParent);   // namerno i idMeeting iz sigurnosnih razloga
        Task<IEnumerable<Diagram>> GetAllAsync();

        Task CreateAsync(Diagram diagram);
        Task UpdateAsync(Diagram diagram);
        Task DeleteAsync(Diagram diagram);
    }
}