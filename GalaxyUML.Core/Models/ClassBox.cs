using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class ClassBox : Box
    {
        public string ClassName { get; private set; }
        public string Stereotype { get; private set; }
        public double TextSize { get; private set; }
        public List<Attribute> AttributeRows { get; private set; }
        public List<Method> MethodRows { get; private set; }

        public ClassBox(/*Guid id, */Point startingPoint, Point endingPoint, Meeting meeting, List<Line> linesStart, List<Line> linesEnd) 
            : base(/*id, */startingPoint, endingPoint, meeting, linesStart, linesEnd)
        {
            base.Type = ObjectType.ClassBox;
            ClassName = "Class";
            Stereotype = "";
            TextSize = 12;
            AttributeRows = new List<Attribute>();
            MethodRows = new List<Method>();
        }

        public void ChangeClassName(string newClassName)
        {
            if (string.IsNullOrWhiteSpace(newClassName))
                throw new ArgumentException("Class name cannot be empty.");

            string stereotype = "";
            string className = newClassName;

            int start = newClassName.IndexOf("<<");
            int end = newClassName.IndexOf(">>");

            if (start != -1 && end != -1 && end > start)
            {
                stereotype = newClassName
                    .Substring(start + 2, end - (start + 2))
                    .Trim();

                className = newClassName
                    .Substring(end + 2)
                    .Trim();

                if (string.IsNullOrWhiteSpace(className))
                    className = "Class";
            }

            Stereotype = stereotype;
            ClassName = className;
        }

        public void ChangeTextSize(double newTextSize) { TextSize = newTextSize; }
        public void AddAttribute(Guid idClassBox, string content, char privacy = '+')
        {
            if (Stereotype == "interface")
                throw new Exception("Cannot add attributes to an interface.");

            AttributeRows.Add(new Attribute(idClassBox, this, privacy + content));
        }
        public void RemoveAttribute(int row) { AttributeRows.RemoveAt(row); }
        public void AddMethod(Guid idClassBox, string content, char privacy = '+')
        {
            if (Stereotype == "enumeration")
                throw new Exception("Can not add methods to an enumeration.");
            
            MethodRows.Add(new Method(idClassBox, this, privacy + content));
        }
        public void RemoveMethod(int row) { MethodRows.RemoveAt(row); }
    }
}