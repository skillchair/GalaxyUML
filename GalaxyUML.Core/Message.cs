namespace GalaxyUML.Core
{
    public class Message
    {
        public Guid IdMessage { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MeetingParticipant Sender { get; private set; }
        public string Content { get; private set; }

        public Message(MeetingParticipant sender, string content)
        {
            IdMessage = Guid.NewGuid();
            Timestamp = DateTime.Now;
            Sender = sender;
            Content = content;
        }
    }
}