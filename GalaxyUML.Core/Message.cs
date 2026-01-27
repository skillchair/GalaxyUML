namespace GalaxyUML.Core
{
    public class Message
    {
        public Guid IdMessage { get; private set; }
        public DateTime Timestamp { get; private set;}
        public string Content { get; private set; }

        public Message(string content)
        {
            IdMessage = Guid.NewGuid();
            Timestamp = DateTime.Now;
            Content = content;
        }
    }
}