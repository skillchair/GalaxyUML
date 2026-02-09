using GalaxyUML.Core.Models;
using Meeting = GalaxyUML.Core.Models.Meeting;

namespace GalaxyUML.Data.Repositories
{
    public interface IMeetingRepo
    {
        Task<Meeting?> GetByIdAsync(Guid id);
        Task<Meeting?> GetByTeamIfActiveAsync(Guid idTeam);
        Task<IEnumerable<Meeting>> GetByTeamAsync(Guid idTeam);
        Task<IEnumerable<Meeting>> GetByOrganizerAsync(Guid idOrganizer);
        Task<IEnumerable<Meeting>> GetAllAsync();

        Task CreateAsync(Meeting meeting, Team team);
        Task DeleteAsync(Meeting meeting);
        Task UpdateAsync(Meeting meeting);
    }
}