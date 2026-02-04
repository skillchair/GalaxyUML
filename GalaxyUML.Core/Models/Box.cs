using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Box : IDrawable
    {
        public List<Line> Lines { get; private set; }

        public Box(Point startingPoint, Point endingPoint, Meeting meeting, List<Line>? lines = null)  // default = null
            : base(startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Box;
            Lines = lines ?? new List<Line>();
        }

        public void LinkLine(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = Lines.FirstOrDefault(l => l.IdDiagram == line.IdDiagram);
            if (lineInAList != null)
                throw new Exception("Line is already connected to this box.");

            Lines.Add(line);
        }

        public void UnlinkLine(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = Lines.FirstOrDefault(l => l.IdDiagram == line.IdDiagram);
            if (lineInAList == null)
                throw new Exception("Line isn't connected to this box.");

            Lines.Remove(lineInAList);
        }

        public override void CleanUp(Diagram parent)
        {
            foreach (var line in Lines.ToList())
            {
                UnlinkLine(line);       // uklanja liniju sa ovog box-a
                line.CleanUp(parent);   // linija se briše iz parent-a
            }
        }

    }
}