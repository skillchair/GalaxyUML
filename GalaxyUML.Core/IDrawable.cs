using System.Drawing;

namespace GalaxyUML.Core
{
    public abstract class IDrawable
    {
        public Guid IdDrawable { get; private set; }
        public DrawableType Type { get; protected set; }
        public Point StartingPoint { get; private set; }
        public Point EndingPoint { get; private set; }

        // mozemo da stavimo i fiksno da ima u konstruktoru podrazumevanu krajnju tacku koja bi se racunala; 
        // tako je to odgovornost modela, ne viseg sloja; neka za sad ovako pa kad odlucimo koje ce dimenzije biti tad
        // I TO RACUNAMO NA OSN TYPE ATRIBUTA I SUPER!
        public IDrawable(Point startingPoint, Point endingPoint)
        {
            IdDrawable = Guid.NewGuid();
            StartingPoint = startingPoint;
            EndingPoint = endingPoint;
        }

        public void Resize(Point newEndingPoint) { EndingPoint = newEndingPoint; }

        // UNESE SE GORNJA LEVA TACKA A DONJA DESNA SE RACUNA
        public void Move(Point newStartingPoint)
        {
            int deltaX = newStartingPoint.X -  StartingPoint.X;
            int deltaY = newStartingPoint.Y - StartingPoint.Y;

            EndingPoint = new Point(EndingPoint.X + deltaX, EndingPoint.Y + deltaY);
            StartingPoint = newStartingPoint;
        }

        public virtual void RemoveSelf()
        {
            return;
        }
    }
}