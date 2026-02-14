namespace GalaxyUML.Core.Models
{
    public class Chat
    {
        private readonly List<Message> _messages = new();
        public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

        public void AddMessage(Guid senderId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Empty message");
            _messages.Add(new Message(Guid.NewGuid(), senderId, content, DateTime.UtcNow));
        }
    }

    public class Message
    {
        public Guid Id { get; }
        public Guid SenderId { get; }
        public string Content { get; }
        public DateTime SentAt { get; }

        public Message(Guid id, Guid senderId, string content, DateTime sentAt)
        { Id = id; SenderId = senderId; Content = content; SentAt = sentAt; }
    }
}
