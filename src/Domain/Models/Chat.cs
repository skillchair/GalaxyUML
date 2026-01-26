namespace GalaxyUML.Models
{
    class Chat
    {
        public Guid IdChat { get; private set; }
        public List<Message> Messages { get; set; }

        public Chat()
        {
            Messages = new List<>();
        }
    }
}