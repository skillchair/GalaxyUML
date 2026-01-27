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

        public void Resize(Point newendingPoint) { EndingPoint = newendingPoint; }

        // moze i da se racuna, nisam siguran sta je bolja praksa
        public void Move(Point newStartingPoint, Point newEndingPoint)
        {
            StartingPoint = newStartingPoint;
            EndingPoint = newEndingPoint;
        }
    }
}