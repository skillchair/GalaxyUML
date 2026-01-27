namespace GalaxyUML.Core
{
    public class Chat
    {
        public Guid IdChat { get; private set; }
        public List<Message> Messages { get; private set; }

        public Chat()
        {
            Messages = new List<Message>();
        }
    }
}