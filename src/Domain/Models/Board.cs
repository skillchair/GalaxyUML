namespace GalaxyUML.Models
{
    public class Board
    {
        public Guid IdBoard { get; set; }
        public List<IDrawable> DrawableObjs { get; set; }
        public Chat Chat { get; set; }

        public Board()
        {
            IdBoard = Guid.NewGuid();
            Chat = new Chat();
        }
    }
}