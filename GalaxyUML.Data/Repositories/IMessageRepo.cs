using Chat = GalaxyUML.Core.Models.Chat;
using Message = GalaxyUML.Core.Models.Message;

namespace GalaxyUML.Data.Repositories
{
    interface IMessageRepo
    {
        Task<Message?> GetByIdAsync(Guid id);
        Task<IEnumerable<Message>> GetAllByChat(Guid idChat);
        Task<IEnumerable<Message>> GetAllByTeamParticipant(Guid idMeetingParticipant);

        Task CreateAsync(Message message, Chat chat);
        // nismo definisali brisanje i edit!!!
    }
}