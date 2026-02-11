namespace GalaxyUML.Core.Models
{
    public class Message
    {
        public Guid IdMessage { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MeetingParticipant Sender { get; private set; }
        public string Content { get; private set; }
        public Guid IdChat { get; private set; }

        public Message(Guid idChat, MeetingParticipant sender, string content)
        {
            IdMessage = Guid.NewGuid();
            Timestamp = DateTime.Now;
            IdChat = idChat;
            Sender = sender;
            Content = content;
        }
    }
}