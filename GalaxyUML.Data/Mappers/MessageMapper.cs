using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;
using Message = GalaxyUML.Core.Models.Message;
using MessageEntity = GalaxyUML.Data.Entities.MessageEntity;

namespace GalaxyUML.Data.Mappers
{
    static class MessageMapper
    {
        public static Message ToModel(MessageEntity entity)
        {
            return new Message(
                entity.IdChat,
                MeetingParticipantMapper.ToModel(entity.Sender),
                entity.Content
            );
        }

        public static MessageEntity ToEntity(Message model/*, ChatEntity chatEntity, TeamEntity teamEntity*/)
        {
            return new MessageEntity
            {
                Id = model.IdMessage,
                IdChat = model.IdChat,
                //Chat = chatEntity,
                IdMeetingParticipant = model.Sender.IdMeetingParticipant,
                Sender = MeetingParticipantMapper.ToEntity(model.Sender/*, teamEntity*/),
                Content = model.Content,
                Timestamp = model.Timestamp
            };
        }
    }
}