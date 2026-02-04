namespace GalaxyUML.Core.Models
{
    public class Attribute
    {
        public Guid IdAttribute { get; private set; }
        public ClassBox ClassBox { get; private set; }
        public string Content { get; private set; }

        public Attribute(ClassBox classBox, string content)
        {
            IdAttribute = Guid.NewGuid();
            ClassBox = classBox;
            Content = content;
        }
    }
}