using System.Drawing;
using System.Text.Json.Serialization;

namespace GalaxyUML.Core.Models
{
    public class Diagram : IDiagram
    {
        public List<IDiagram> Objs { get; private set; }

        [JsonConstructor] // Kažeš JSON-u: "Koristi BAŠ ovaj konstruktor"
        public Diagram(/*Guid id, */Point startingPoint, Point endingPoint, Meeting meeting) 
                : base(/*id, */startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Diagram;
            Objs = new List<IDiagram>();
        }

        public Diagram(/*Guid id, */Point startingPoint, Point endingPoint, Meeting meeting, List<IDiagram> objs)
    : base(/*id, */startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Diagram;
            Objs = objs ?? new List<IDiagram>();
        }

        public void Add(IDiagram obj)
        {
            var objInAList = Objs.FirstOrDefault(obj);
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
            var objInAList = Objs.FirstOrDefault(obj);
            if (objInAList == null)
                throw new Exception("Object not on this diagram.");

            obj.CleanUp(this);  // da se unlinkuje ako je linija ili kutija                
            Objs.Remove(obj);
        }
    }
}