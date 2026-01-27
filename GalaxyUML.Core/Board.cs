using System.Diagnostics.Contracts;

namespace GalaxyUML.Core
{
    public class Board
    {
        public Guid IdBoard { get; set; }
        public List<Diagram> Diagrams { get; private set; }
        public List<IDrawable> Drawables { get; private set; }

        public Board()
        {
            IdBoard = Guid.NewGuid();
            Diagrams = new List<Diagram>();
            Drawables = new List<IDrawable>();
        }

        public void AddDiagram(Diagram diagram)
        {
            var diagramInAList = Diagrams.FirstOrDefault(d => d.IdDiagram == diagram.IdDiagram);
            if (diagramInAList != null)
                throw new Exception("Diagram already on this board.");

            Diagrams.Add(diagram);
        }

        public void RemoveDiagram(Diagram diagram)
        {
            var diagramInAList = Diagrams.FirstOrDefault(d => d.IdDiagram == diagram.IdDiagram);
            if (diagramInAList == null)
                throw new Exception("Diagram not on this board.");

            Diagrams.Remove(diagram);
        }

        public void AddDrawable(IDrawable drawable)
        {
            var drawableInAList = Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("Drawable object already on this diagram.");

            Drawables.Add(drawable);
        }
        public void RemoveDrawable(IDrawable drawable)
        {
            var drawableInAList = Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("Drawable object not on this diagram.");

            Drawables.Remove(drawable);
        }
    }
}