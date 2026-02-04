namespace GalaxyUML.Core.Models
{
    public class Method
    {
        public Guid IdMethod { get; private set; }
        public ClassBox ClassBox { get; private set; }
        public string Content { get; private set; }

        public Method(ClassBox classBox, string content)
        {
            IdMethod = Guid.NewGuid();
            ClassBox = classBox;
            Content = content;
        }
    }
}