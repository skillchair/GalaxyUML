using Message = GalaxyUML.Core.Models.Message;
using MessageEntity = GalaxyUML.Data.Entities.MessageEntity;
using Chat = GalaxyUML.Core.Models.Chat;
using ChatEntity = GalaxyUML.Data.Entities.ChatEntity;

namespace GalaxyUML.Data.Mappers
{
    static class ChatMapper
    {
        public static Chat ToModel(ChatEntity entity)
        {
            return new Chat(
                MeetingMapper.ToModel(entity.Meeting)
            );
        }

        public static ChatEntity ToEntity(Chat model)
        {
            List<MessageEntity> messages = new List<MessageEntity>();
            foreach (var msg in model.Messages)
                messages.Add(MessageMapper.ToEntity(msg, model));

            return new ChatEntity
            {
                Id = model.IdChat,
                IdMeeting = model.Meeting.IdMeeting,
                Meeting = MeetingMapper.ToEntity(model.Meeting),
                Messages = messages
            };
        }
    }
}