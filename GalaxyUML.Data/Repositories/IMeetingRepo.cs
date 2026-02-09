using GalaxyUML.Core.Models;
using Meeting = GalaxyUML.Core.Models.Meeting;

namespace GalaxyUML.Data.Repositories
{
    public interface IMeetingRepo
    {
        Task<Meeting?> GetByIdAsync(Guid id);
        Task<Meeting?> GetByTeamIfActiveAsync(Guid idTeam);
        Task<IEnumerable<Meeting>> GetAllByTeamAsync(Guid idTeam);
        Task<IEnumerable<Meeting>> GetAllByOrganizerAsync(Guid idOrganizer);

        Task CreateAsync(Meeting meeting, Team team);
        Task DeleteAsync(Meeting meeting);
        Task UpdateAsync(Meeting meeting);
    }
}