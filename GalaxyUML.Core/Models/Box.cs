using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Box : IDrawable
    {
        public List<Line> LinesAsStart { get; private set; }
        public List<Line> LinesAsEnd { get; private set; }

        public Box(/*Guid id, */Point startingPoint, Point endingPoint, Meeting meeting, List<Line> linesStart, List<Line> linesEnd)  // default = null
            : base(/*id, */startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Box;
            LinesAsStart = linesStart ?? new List<Line>();
            LinesAsEnd = linesEnd ?? new List<Line>();
        }

        public void LinkLineStart(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = LinesAsStart.FirstOrDefault(line);
            if (lineInAList != null)
                throw new Exception("Line is already connected to this box.");

            LinesAsStart.Add(line);
        }
        public void LinkLineEnd(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = LinesAsEnd.FirstOrDefault(line);
            if (lineInAList != null)
                throw new Exception("Line is already connected to this box.");

            LinesAsEnd.Add(line);
        }

        public void UnlinkLineStart(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = LinesAsStart.FirstOrDefault(line);
            if (lineInAList == null)
                throw new Exception("Line isn't connected to this box.");

            LinesAsStart.Remove(lineInAList);
        }

        public void UnlinkLineEnd(Line line)
        {
            if (line == null)
                throw new Exception("Line doesn't exist.");

            var lineInAList = LinesAsEnd.FirstOrDefault(line);
            if (lineInAList == null)
                throw new Exception("Line isn't connected to this box.");

            LinesAsEnd.Remove(lineInAList);
        }

        public override void CleanUp(Diagram parent)
        {
            foreach (var line in LinesAsStart.ToList())
            {
                UnlinkLineStart(line);       // uklanja liniju sa ovog box-a
                line.CleanUp(parent);   // linija se briše iz parent-a
            }
            foreach (var line in LinesAsEnd.ToList())
            {
                UnlinkLineEnd(line);       // uklanja liniju sa ovog box-a
                line.CleanUp(parent);   // linija se briše iz parent-a
            }
        }

    }
}