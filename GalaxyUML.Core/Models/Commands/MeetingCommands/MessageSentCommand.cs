namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public class MessageSentCommand : IMeetingCommand
    {
        public Chat Chat { get; private set; }
        public Message Message { get; private set; }

        public MessageSentCommand(Meeting meeting, Chat chat, Message message) : base(meeting)
        {
            Chat = chat;
            Message = message;
        }

        public override void Execute(MeetingEventType eventType)
        {
            if (eventType != MeetingEventType.MessageSent)
                throw new Exception("Invalid event. Expected MeetingEventType.MessageSent.");

            Chat.AddMessage(Message);
        }
    }
}