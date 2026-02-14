using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public abstract class IDrawable : IDiagram
    {
        protected IDrawable(ObjectType type, Point start, Point end, Diagram parent)
            : base(type, start, end, parent) { }
    }
}
