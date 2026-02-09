using MeetingParticipant = GalaxyUML.Core.Models.MeetingParticipant;

namespace GalaxyUML.Data.Repositories
{
    interface IMeetingParticipantRepo
    {
        Task<MeetingParticipant?> GetByIdAsync(Guid id);
        Task<IEnumerable<MeetingParticipant>> GetByMeetingAsync(Guid idMeeting);
        Task<IEnumerable<MeetingParticipant>> GetByParticipantAsync(Guid idParticipant);
        Task<IEnumerable<MeetingParticipant>> GetAllAsync();

        Task CreateAsync(MeetingParticipant participant);
        Task DeleteAsync(MeetingParticipant participant);
        Task UpdateAsync(MeetingParticipant participant);
    }
}