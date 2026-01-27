using System.Drawing;

namespace GalaxyUML.Core
{
    public class ClassBox : IDrawable
    {
        public string ClassName { get; private set; }
        public string Stereotype { get; private set; }
        public double TextSize { get; private set; }
        public List<string> AttributeRows { get; private set; }
        public List<string> MethodRows { get; private set; }

        public ClassBox(DrawableType type, Point startingPoint, Point endingPoint, double textSize) : base(type, startingPoint, endingPoint)
        {
            ClassName = "Class";
            Stereotype = "";
            TextSize = textSize;
            AttributeRows = new List<string>();
            MethodRows = new List<string>();
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
        public void AddAttribute(string content, char privacy = '+') { AttributeRows.Add(privacy + content); }
        public void RemoveAttribute(int row) { AttributeRows.RemoveAt(row); }
        public void AddMethod(string content, char privacy = '+')
        {
            if (Stereotype == "enumeration")
                throw new Exception("Can not add methods to an enumeration.");
            
            AttributeRows.Add(privacy + content);
        }
        public void RemoveMethod(int row) { AttributeRows.RemoveAt(row); }
    }
}