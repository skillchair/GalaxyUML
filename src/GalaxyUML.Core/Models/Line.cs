namespace GalaxyUML.Core.Models
{
    public class Line : IDrawable
    {
        public Guid BoxId { get; }
        public string? MiddleText { get; private set; }
        public string? Text1 { get; private set; }
        public string? Text2 { get; private set; }

        internal Line(Box box, string? middle, string? t1, string? t2)
            : base(ObjectType.Line, box.StartingPoint, box.EndingPoint, box.Parent!)
        {
            BoxId = box.Id;
            MiddleText = middle; Text1 = t1; Text2 = t2;
        }

        public void UpdateTexts(string? middle, string? t1, string? t2)
        { MiddleText = middle; Text1 = t1; Text2 = t2; }

        public override void OnRemovedFromParent()
        {
            Parent!.Detach(BoxId);
        }
    }
}
