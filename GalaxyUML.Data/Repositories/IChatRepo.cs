using Chat = GalaxyUML.Core.Models.Chat;

namespace GalaxyUML.Data.Repositories
{
    public interface IChatRepo
    {
        Task<Chat?> GetByIdAsync(Guid id);
        Task<Chat?> GetByMeetingAsync(Guid idMeeting);
        Task<IEnumerable<Chat>> GetAllAsync();

        Task CreateAsync(Chat chat/*, Team team*/);
        //Task UpdateAsync(Guid id, Chat chat);
        //Task DeleteAsync(Guid id);
    }
}