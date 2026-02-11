using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public abstract class IDrawable : IDiagram
    {
        // mozemo da stavimo i fiksno da ima u konstruktoru podrazumevanu krajnju tacku koja bi se racunala; 
        // tako je to odgovornost modela, ne viseg sloja; neka za sad ovako pa kad odlucimo koje ce dimenzije biti tad
        // I TO RACUNAMO NA OSN TYPE ATRIBUTA I SUPER!
        public IDrawable(/*Guid id, */Point startingPoint, Point endingPoint, Meeting meeting)  // default = null
            : base(/*id, */startingPoint, endingPoint, meeting)
        {
        }

        public override void Resize(Point newEndingPoint) { EndingPoint = newEndingPoint; }

        // UNESE SE GORNJA LEVA TACKA A DONJA DESNA SE RACUNA
        public override void Move(Point newStartingPoint)
        {
            StartingPoint = newStartingPoint;
            int deltaX = newStartingPoint.X - StartingPoint.X;
            int deltaY = newStartingPoint.Y - StartingPoint.Y;

            Point newEndingPoint = new Point(EndingPoint.X + deltaX, EndingPoint.Y + deltaY);

            // int deltaX = newStartingPoint.X - StartingPoint.X;
            // int deltaY = newStartingPoint.Y - StartingPoint.Y;

            // Point newEndingPoint = new Point(EndingPoint.X + deltaX, EndingPoint.Y + deltaY);
            // if (Parent == null)
            // {
            //     StartingPoint = newStartingPoint;
            //     EndingPoint = newEndingPoint;
            // }
            // else
            // {
            //     // obezbedjujemo se da ne pobegne iz granica parent-a!!!
            //     if (newStartingPoint.X < Parent.StartingPoint.X || newStartingPoint.Y < Parent.StartingPoint.Y ||
            //     newEndingPoint.X > Parent.EndingPoint.X || newEndingPoint.Y > Parent.EndingPoint.Y)
            //         throw new Exception("Drawable out of diagram's bounds.");

            //     Parent.Move(newStartingPoint);
            //  }
        }
    }
}