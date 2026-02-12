using System.Text.Json.Serialization;

namespace GalaxyUML.Core.Models
{
    public class Message
    {
        public Guid IdMessage { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MeetingParticipant Sender { get; private set; }
        public Guid IdSender { get; private set; }
        public string Content { get; private set; }
        public Guid IdChat { get; private set; }

        [JsonConstructor] // Kažeš JSON-u: "Koristi BAŠ ovaj konstruktor"
        public Message(Guid idChat, Guid idSender, MeetingParticipant sender, string content)
        {
            IdMessage = Guid.NewGuid();
            Timestamp = DateTime.Now;
            IdChat = idChat;
            IdSender = idSender;
            Sender = sender;
            Content = content;
        }
    }
}