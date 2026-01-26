using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

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
            // chat = new chat()
        }
    }
}