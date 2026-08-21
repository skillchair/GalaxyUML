using System.Drawing;

namespace GalaxyUML.Core.Models.Constants
{
    public static class Constants
    {
        private const int MinCoordinateX = 0;
        private const int MinCoordinateY = 0;
        private const int MaxCoordinateX = 1000;
        private const int MaxCoordinateY = 1000;
        public static readonly Point MinPoint = new Point(MinCoordinateX, MinCoordinateY);
        public static readonly Point MaxPoint = new Point(MaxCoordinateX, MaxCoordinateY);
    }
}