using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Box : IDrawable
    {
        private readonly List<Line> _lines = new();
        public IReadOnlyCollection<Line> Lines => _lines.AsReadOnly();

        public Box(Point start, Point end, Diagram parent)
            : base(ObjectType.Box, start, end, parent) { }

        public Line AddLine(Line line)
        {
            if (line.Parent != this.Parent)
                throw new Exception("Can not connect a line from a different diagram.");

            _lines.Add(line);
            return line;
        }

        public void RemoveLine(Guid lineId) => _lines.RemoveAll(l => l.Id == lineId);

        public override void OnRemovedFromParent()
        {
            foreach (var l in _lines)
                l.Parent!.Detach(l.Id);
        }
    }
}
