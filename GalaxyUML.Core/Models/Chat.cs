namespace GalaxyUML.Core.Models
{
    public class Chat
    {
        //public Guid IdChat { get; private set; }
        public List<Message> Messages { get; private set; }
        public Meeting Meeting { get; private set; }
        public Guid IdMeeting { get; private set; }

        public Chat(/*Guid id, */Guid idMeeting, Meeting meeting)
        {
            //IdChat = id;
            IdMeeting = idMeeting;
            Messages = new List<Message>();
            Meeting = meeting;
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