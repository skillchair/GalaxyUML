using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Diagram : IDiagram
    {
        public List<IDiagram> Objs { get; private set; }

        public Diagram(Meeting meeting): base(meeting)
        {
            base.Type = ObjectType.Diagram;
            Objs = new List<IDiagram>();
        }

        public Diagram(Point startingPoint, Point endingPoint, Meeting meeting, List<IDiagram>? objs = null)  // default = null
    : base(startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Diagram;
            Objs = objs ?? new List<IDiagram>();
        }

        public void Add(IDiagram obj)
        {
            var objInAList = Objs.FirstOrDefault(o => o.IdDiagram == obj.IdDiagram);
            if (objInAList == null)
                throw new Exception("Object already on this diagram.");

            // ne sme da bude van dijagrama
            if (obj.StartingPoint.X > StartingPoint.X || obj.StartingPoint.Y > StartingPoint.Y
                || obj.EndingPoint.X > EndingPoint.X || obj.EndingPoint.Y > EndingPoint.Y)
                throw new Exception("Can't add object out of parent's bounds.");

            Objs.Add(obj);
        }

        public override void Move(Point newStartingPoint)
        {
            foreach (var obj in Objs)
                obj.Move(newStartingPoint);
        }

        public override void Resize(Point newEndingPoint)
        {
            foreach (var obj in Objs)
                obj.Resize(newEndingPoint);
        }

        public override void CleanUp(Diagram parent)
        {
            foreach (var obj in Objs)
                obj.CleanUp(parent);
        }

        public void Remove(IDiagram obj)
        {
            var objInAList = Objs.FirstOrDefault(o => o.IdDiagram == obj.IdDiagram);
            if (objInAList == null)
                throw new Exception("Object not on this diagram.");

            obj.CleanUp(this);  // da se unlinkuje ako je linija ili kutija                
            Objs.Remove(obj);
        }
    }
}