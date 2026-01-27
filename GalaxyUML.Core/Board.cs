namespace GalaxyUML.Core
{
    public class Board
    {
        public Guid IdBoard { get; set; }
        public List<IDrawable> DrawableObjs { get; set; }
        public Chat Chat { get; private set; }

        public Board()
        {
            IdBoard = Guid.NewGuid();
            DrawableObjs = new List<IDrawable>();
            Chat = new Chat();
        }
    }
}