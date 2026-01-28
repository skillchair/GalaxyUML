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

        public void AddDrawableToADiagram(IDrawable drawable, Diagram diagram)
        {
            var drawableInAList = diagram.Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList != null)
                throw new Exception("This object is already on this board.");

            diagram.AddDrawable(drawable);
        }

        public void RemoveDrawableFromADiagram(IDrawable drawable, Diagram diagram)
        {
            var drawableInAList = diagram.Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("This object isn't on this board.");

            diagram.RemoveDrawable(drawable);
        }

        public void EditDrawable(IDrawable drawable, IDrawable newDrawable)
        {
            var drawableInAList = Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("This object isn't on this board.");

            Drawables.Remove(drawableInAList);
            Drawables.Add(newDrawable);
        }

        public void EditDrawableOnADiagram(IDrawable drawable, IDrawable newDrawable, Diagram diagram)
        {
            var drawableInAList = Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("This object isn't on this board.");

            var diagramInAList = Diagrams.FirstOrDefault(d => d.IdDiagram == diagram.IdDiagram);
            if (diagramInAList == null)
                throw new Exception("Diagram not on this board.");

            diagram.RemoveDrawable(drawable);
            diagram.AddDrawable(newDrawable);
        }
    }
}