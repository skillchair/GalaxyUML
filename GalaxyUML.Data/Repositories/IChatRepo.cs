using Chat = GalaxyUML.Core.Models.Chat;
using Team = GalaxyUML.Core.Models.Team;

namespace GalaxyUML.Data.Repositories
{
    interface IChatRepo
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<Chat?> GetByMeetingAsync(Guid idMeeting);
        Task<IEnumerable<Chat>> GetAllAsync();

        Task CreateAsync(Chat chat, Team team);
        Task DeleteAsync(Chat chat);
        Task UpdateAsync(Chat chat);
    }
}