using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public abstract class IDiagram
    {
        public Guid IdDiagram { get; private set; }
        public ObjectType Type { get; protected set; }
        public Point StartingPoint { get; protected set; }
        public Point EndingPoint { get; protected set; }
        public Meeting Meeting { get; private set; }
        public Guid IdParent { get; private set; }

        public IDiagram(Meeting meeting, Guid? idParent = null)
        {
            StartingPoint = Constants.Constants.MinPoint;
            EndingPoint = Constants.Constants.MaxPoint;
            Meeting = meeting;
            IdParent = idParent ?? IdParent;
        }

        public IDiagram(Point startingPoint, Point endingPoint, Meeting meeting)
        {
            IdDiagram = Guid.NewGuid();
            StartingPoint = startingPoint;
            EndingPoint = endingPoint;
            Meeting = meeting;
        }

        // nek ostane za sad da nemamo proveru za out of bounds... videcemo kako cemo
        public abstract void Move(Point newStartingPoint);
        public abstract void Resize(Point newEndingPoint);
        public abstract void CleanUp(Diagram parent);
    }
}