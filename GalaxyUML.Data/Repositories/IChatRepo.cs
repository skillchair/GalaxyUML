using GalaxyUML.Core.Models;
using Chat = GalaxyUML.Core.Models.Chat;

namespace GalaxyUML.Data.Repositories
{
    interface IChatRepo
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<Chat?> GetByMeetingAsync(Guid idMeeting);

        Task CreateAsync(Chat chat, Team team);
        Task DeleteAsync(Chat chat);
        Task UpdateAsync(Chat chat);
    }
}