using MeetingParticipant = GalaxyUML.Core.Models.MeetingParticipant;
using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Data.Repositories
{
    public interface IMeetingParticipantRepo
    {
        Task<MeetingParticipant?> GetByIdAsync(Guid id);
        Task<IEnumerable<MeetingParticipant>> GetByMeetingAsync(Guid idMeeting);
        Task<IEnumerable<MeetingParticipant>> GetByParticipantAsync(Guid idParticipant);
        Task<IEnumerable<MeetingParticipant>> GetAllAsync();

        Task CreateAsync(MeetingParticipant participant, Team team);
        Task UpdateAsync(MeetingParticipant participant);
        Task DeleteAsync(Guid id);
    }
}