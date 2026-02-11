namespace GalaxyUML.Core.Models
{
    public class Method
    {
        //public Guid IdMethod { get; private set; }
        public Guid IdClassBox { get; private set; }
        public ClassBox ClassBox { get; private set; }
        public string Content { get; private set; }

        public Method(Guid idClassBox, ClassBox classBox, string content)
        {
            //IdMethod = Guid.NewGuid();
            IdClassBox = idClassBox;
            ClassBox = classBox;
            Content = content;
        }
    }
}