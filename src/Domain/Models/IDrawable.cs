using System.Drawing;

namespace GalaxyUML.Models
{
    public abstract class IDrawable
    {
        public Guid IdDrawable { get; set; }
        public DrawableType Type { get; set; }
        public Point StartingPoint { get; set; }
        public Point EndingPoint { get; set; }

        public  IDrawable(DrawableType type, StartingPoint startingPoint, EndingPoint endingPoint)
        {
            IdDrawable = Guid.NewGuid();
            Type = type;
            StartingPoint = startingPoint;
            EndingPoint = endingPoint;
        }

        public void Draw()
        {
            
        }
    }
}