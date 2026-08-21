using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Diagram : IDiagram
    {
        private readonly List<IDiagram> _children = new();
        public Diagram() : base(ObjectType.Diagram, Constants.Constants.MinPoint, Constants.Constants.MaxPoint) { }
        public Diagram(Point start, Point end) : base(ObjectType.Diagram, start, end) { }
        // Only resize/move allowed; no extra state

        public void Attach(Diagram child)
        {
            child.Parent = this;
            _children.Add(child);
        }

        public void Detach(Guid childId)
        {
            var c = _children.FirstOrDefault(c => c.Id == childId);
            if (c != null)
            {
                c.OnRemovedFromParent();
                _children.Remove(c);
            }
        }

        public override void Move(Point newTopLeft)
        {
            var dx = newTopLeft.X - StartingPoint.X;
            var dy = newTopLeft.Y - StartingPoint.Y;
            StartingPoint = newTopLeft;
            EndingPoint = new Point(EndingPoint.X + dx, EndingPoint.Y + dy);
            foreach (var c in _children) c.Move(new Point(c.StartingPoint.X + dx, c.StartingPoint.Y + dy));

            base.Move(newTopLeft);
        }

        public override void Resize(Point newBottomRight)
        {
            foreach (var c in _children)
            {
                var difX = EndingPoint.X - newBottomRight.X;
                var difY = EndingPoint.Y - newBottomRight.Y;

                Point newBottomRightC = new Point(c.EndingPoint.X + difX, c.EndingPoint.Y + difY);
                c.Resize(newBottomRightC);
            }
            base.Resize(newBottomRight);
        }

        public override void OnRemovedFromParent()
        {
            foreach (var c in _children)
                c.OnRemovedFromParent();
        }
    }
}
