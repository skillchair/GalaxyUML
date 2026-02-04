using System.Drawing;

namespace GalaxyUML.Core
{
    public class Diagram
    {
        public Guid IdDiagram { get; private set; }
        public Point StartingPoint { get; private set; }
        public Point EndingPoint { get; private set; }
        public List<IDrawable> Drawables { get; private set; }

        public Diagram()
        {
            IdDiagram = Guid.NewGuid();
            StartingPoint = new Point(0, 0);
            EndingPoint = new Point(1000, 1000);
            Drawables = new List<IDrawable>();
        }
        public Diagram(Point startingPoint, Point endingPoint)
        {
            IdDiagram = Guid.NewGuid();
            StartingPoint = startingPoint;
            EndingPoint = endingPoint;
            Drawables = new List<IDrawable>();
        }

        public void AddDrawable(IDrawable drawable)
        {
            var drawableInAList = Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("Drawable object already on this diagram.");

            // ne sme da bude van dijagrama
            if (drawable.StartingPoint.X > StartingPoint.X || drawable.StartingPoint.Y > StartingPoint.Y
                || drawable.EndingPoint.X > EndingPoint.X || drawable.EndingPoint.Y > EndingPoint.Y)
                throw new Exception("Drawable out of diagram's bounds.");

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

        public void MoveDrawable(IDrawable drawable, Point newStartingPoint)
        {
            var drawableInAList = Drawables.FirstOrDefault(d => d.IdDrawable == drawable.IdDrawable);
            if (drawableInAList == null)
                throw new Exception("Drawable object not on this diagram.");

            int deltaX = newStartingPoint.X - drawable.StartingPoint.X;
            int deltaY = newStartingPoint.Y - drawable.StartingPoint.Y;

            Point newEndingPoint = new Point(drawable.EndingPoint.X + deltaX, drawable.EndingPoint.Y + deltaY);
            
            if (newStartingPoint.X < StartingPoint.X || newStartingPoint.Y < StartingPoint.Y ||
                newEndingPoint.X > EndingPoint.X || newEndingPoint.Y > EndingPoint.Y)
                throw new Exception("Drawable out of diagram's bounds.");

            drawable.Move(newStartingPoint);
        }

        // RESIZE SE UVEK RADI OD DONJE DESNE IVICE!
        // neophodna je provera da ne ispadne IDrawable objekat
        public void ResizeDiagram(Point newEndingPoint)
        {
            // DORADI
            // nadjemo najdalje
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach(IDrawable drawable in Drawables)
            {
                if (drawable.EndingPoint.X > maxX)
                    maxX = drawable.EndingPoint.X;
                if (drawable.EndingPoint.Y > maxY)
                    maxY = drawable.EndingPoint.Y;
            }

            if (newEndingPoint.X < maxX || newEndingPoint.Y < maxY)
                throw new Exception("Diagram would be unable to fit all of it's subelements.");

            EndingPoint = newEndingPoint;

        }
        // dozvoljavamo preklapanje dijagrama
        // kad se korisnik zezne nek uradi undo, pp da nije glup da preklopi 2 dijagrama
        // + bice mu korisno da ga utanaci
        public void MoveDiagram(Point newStartingPoint)
        {
            int deltaX = newStartingPoint.X -  StartingPoint.X;
            int deltaY = newStartingPoint.Y - StartingPoint.Y;

            EndingPoint = new Point(EndingPoint.X + deltaX, EndingPoint.Y + deltaY);
            StartingPoint = newStartingPoint;
            
            // da se i podelementi mrdnu
            foreach (IDrawable drawable in Drawables)
                drawable.Move(newStartingPoint);
        }
    }
}