using GalaxyUML.Core.Models;
using Message = GalaxyUML.Core.Models.Message;
using MessageEntity = GalaxyUML.Data.Entities.MessageEntity;

namespace GalaxyUML.Data.Mappers
{
    static class MessageMapper
    {
        public static Message ToModel(MessageEntity entity)
        {
            return new Message(
                MeetingParticipantMapper.ToModel(entity.Sender),
                entity.Content
            );
        }

        public static MessageEntity ToEntity(Message model, Chat chat)
        {
            return new MessageEntity
            {
                Id = model.IdMessage,
                IdChat = chat.IdChat,
                Chat = ChatMapper.ToEntity(chat),
                IdMeetingParticipant = model.Sender.IdMeetingParticipant,
                Sender = MeetingParticipantMapper.ToEntity(model.Sender),
                Content = model.Content,
                Timestamp = model.Timestamp
            };
        }
    }
}