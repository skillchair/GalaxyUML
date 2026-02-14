using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Text : IDrawable
    {
        public string Content { get; private set; } = "";
        public string? Format { get; private set; }
        public int FontSize { get; private set; } = 14;
        public string Color { get; private set; } = "#000";

        public Text(Point start, Point end, Diagram parent)
            : base(ObjectType.Text, start, end, parent) { }

        public void Update(string content, int fontSize, string color, string? format)
        { Content = content; FontSize = fontSize; Color = color; Format = format; }
    }
}
