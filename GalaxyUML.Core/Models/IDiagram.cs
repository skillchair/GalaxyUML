using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public abstract class IDiagram
    {
        public Guid Id { get; }
        public ObjectType Type { get; protected set; }
        //public Guid? ParentId { get; private set; }
        public Diagram? Parent { get; protected set; }
        //public IReadOnlyCollection<IDiagram> Children => _children.AsReadOnly();
        public Point StartingPoint { get; protected set; }
        public Point EndingPoint { get; protected set; }

        protected IDiagram(ObjectType type, Point start, Point end, Diagram? parent = null)
        {
            Id = Guid.NewGuid();
            Type = type;
            StartingPoint = start;
            EndingPoint = end;
            Parent = parent;
            //ParentId = parent?.Id;
        }

        //public void SetParent(IDiagram element, Diagram parent) { element.Parent = parent; }

        public virtual void Move(Point newTopLeft)
        {
            StartingPoint = new Point(newTopLeft.X, newTopLeft.Y);

            var difX = StartingPoint.X - newTopLeft.X;
            var difY = StartingPoint.Y - newTopLeft.Y;
            EndingPoint = new Point(EndingPoint.X + difX, EndingPoint.Y + difY);
        }

        public virtual void Resize(Point newBottomRight) => EndingPoint = newBottomRight;

        public virtual void OnRemovedFromParent() {}
    }
}
