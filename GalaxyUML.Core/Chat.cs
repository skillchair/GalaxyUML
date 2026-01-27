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

        public void AddMessage(Message message)
        {
            var msg = Messages.FirstOrDefault(m => m.IdMessage == message.IdMessage);
            if (msg != null)
                throw new Exception("This message is already in this chat.");
                
            Messages.Add(message);
        }
    }
}