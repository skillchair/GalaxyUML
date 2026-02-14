using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class ChatMapper
{
    public static Chat ToDomain(ChatEntity e)
    {
        var chat = new Chat();
        foreach (var msg in e.Messages)
            chat.AddMessage(msg.SenderId, msg.Content); // SentAt kept by entity
        return chat;
    }

    public static ChatEntity ToEntity(Guid meetingId, Chat d)
    {
        var chat = new ChatEntity { Id = Guid.NewGuid(), MeetingId = meetingId };
        chat.Messages = d.Messages.Select(m => MessageMapper.ToEntity(chat.Id, m)).ToList();
        return chat;
    }
}
