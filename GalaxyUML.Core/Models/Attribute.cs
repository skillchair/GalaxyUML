namespace GalaxyUML.Core.Models
{
    public class Attribute
    {
        //public Guid IdAttribute { get; private set; }
        public ClassBox ClassBox { get; private set; }
        public string Content { get; private set; }
        public Guid IdClassBox { get; private set; }

        public Attribute(/*Guid id, */Guid idClassBox, ClassBox classBox, string content)
        {
            //IdAttribute = id;
            IdClassBox = idClassBox;
            ClassBox = classBox;
            Content = content;
        }
    }
}