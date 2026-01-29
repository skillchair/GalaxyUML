using System.Drawing;

namespace GalaxyUML.Core
{
    public class Box : IDrawable
    {
        public Line? Line { get; private set; }

        public Box(Point startingPoint, Point endingPoint) : base(startingPoint, endingPoint)
        {
            base.Type = DrawableType.Box;
            Line = null;
        }

        public void LinkLine(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            Line = line;
        }

        public void UnlinkLine() { Line = null; }

        override public void RemoveSelf()
        {
            if (Line == null)
                throw new Exception("Box isn't connected to a line.");

            Line.RemoveSelf();
            Line = null;    // box mora da zna da je ostao bez line-a
        }
    }
}