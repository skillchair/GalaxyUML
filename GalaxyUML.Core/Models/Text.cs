using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Text : IDrawable
    {
        public string Content { get; private set; }
        public string Format { get; private set; }
        public double Size { get; private set; }    // wpf koristi double
        public string Color { get; private set; }   // HEXA ZA WPF

        public Text(Point startingPoint, Point endingPoint, Meeting meeting,
                    string content, string format, double size, string color)  // default = null
            : base(startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Text;
            Content = content;
            Format = format;
            Size = size;
            Color = color;
        }

        public void ChangeTextContent(string content) { Content = content; }
        public void ChangeTextFormat(string format) { Format = format; }
        public void ChangeTextSize(double size) { Size = size; }
        public void ChangeTextColor(string color) { Color = color; }

        public override void CleanUp(Diagram parent)
        {
            return;
        }
    }
}