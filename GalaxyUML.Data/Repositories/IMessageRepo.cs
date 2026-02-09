using Chat = GalaxyUML.Core.Models.Chat;
using Message = GalaxyUML.Core.Models.Message;

namespace GalaxyUML.Data.Repositories
{
    interface IMessageRepo
    {
        Task<Message?> GetByIdAsync(Guid id);
        Task<IEnumerable<Message>> GetByChatAsync(Guid idChat);
        Task<IEnumerable<Message>> GetByTeamParticipantAsync(Guid idMeetingParticipant);
        Task<IEnumerable<Message>> GetAllAsync();

        Task CreateAsync(Message message, Chat chat);
        // nismo definisali brisanje i edit!!!
    }
}