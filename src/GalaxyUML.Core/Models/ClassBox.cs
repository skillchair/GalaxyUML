using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class ClassBox : Box
    {
        private readonly List<string> _attributes = new();
        private readonly List<string> _methods = new();

        public IReadOnlyCollection<string> Attributes => _attributes.AsReadOnly();
        public IReadOnlyCollection<string> Methods => _methods.AsReadOnly();

        public ClassBox(Point start, Point end, Diagram parent)
            : base(start, end, parent) { Type = ObjectType.ClassBox; }

        public void AddAttribute(string name) => _attributes.Add(name);
        public void RemoveAttribute(string name) => _attributes.Remove(name);
        public void AddMethod(string signature) => _methods.Add(signature);
        public void RemoveMethod(string signature) => _methods.Remove(signature);
    }
}
