namespace GalaxyUML.Core
{
    public class Diagram
    {
        public Guid IdDiagram { get; private set; }
        public List<IDrawable> Drawables { get; private set; }

        public Diagram()
        {
            IdDiagram = Guid.NewGuid();
            Drawables = new List<IDrawable>();
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

            drawable.RemoveSelf();  // da se unlinkuje ako je linija ili kutija                
            Drawables.Remove(drawable);
        }
    }
}