using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class MessageMapper
{
    public static Message ToDomain(MessageEntity e) =>
        new Message(e.Id, e.SenderId, e.Content, e.SentAt);

    public static MessageEntity ToEntity(Guid chatId, Message d) => new()
    {
        Id = d.Id,
        ChatId = chatId,
        SenderId = d.SenderId,
        Content = d.Content,
        SentAt = d.SentAt
    };
}
