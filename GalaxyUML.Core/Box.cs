using System.Drawing;

namespace GalaxyUML.Core
{
    public class Box : IDrawable
    {
        public Box(Point startingPoint, Point endingPoint) : base(startingPoint, endingPoint)
        {
            base.Type = DrawableType.Box;
        }
    }
}