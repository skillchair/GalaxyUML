using System.Drawing;

namespace GalaxyUML.Core
{
    public class Box : IDrawable
    {
        public List<Line> Lines { get; private set; }

        public Box(Point startingPoint, Point endingPoint) : base(startingPoint, endingPoint)
        {
            base.Type = DrawableType.Box;
            Lines = new List<Line>();
        }

        public void LinkLine(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");
            
            var lineInAList = Lines.FirstOrDefault(l => l.IdDrawable == line.IdDrawable);
            if (lineInAList != null)
                throw new Exception("Line is already connected to this box.");

            Lines.Add(lineInAList);
        }

        public void UnlinkLine(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = Lines.FirstOrDefault(l => l.IdDrawable == line.IdDrawable);
            if (lineInAList == null)
                throw new Exception("Line isn't connected to this box.");

            Lines.Remove(lineInAList);
        }

        override public void RemoveSelf()
        {
            foreach (Line line in Lines)
            {
                Line.RemoveSelf();
                this.UnlinkLine(line);
            }
        }
    }
}