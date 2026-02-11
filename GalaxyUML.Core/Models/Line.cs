using System.Drawing;

namespace GalaxyUML.Core.Models
{
    public class Line : IDrawable      // PRETPOSTAVLJAMO DA JE SMER UVEK OD CLASS1 KA CLASS2 DA NE BISMO DODATNO PAMTILI SMER!
    {
        public Box Box1 { get; private set; }
        public Box Box2 { get; private set; }
        public string Text1 { get; private set; }
        public string Text2 { get; private set; }
        public string MiddleText { get; private set; }

        public Line(/*Guid id, */Point startingPoint, Point endingPoint, Meeting meeting,
                    Box box1, Box box2)  // default = null
            : base(/*id, */startingPoint, endingPoint, meeting)
        {
            base.Type = ObjectType.Line;
            Box1 = box1;
            Box2 = box2;
            Text1 = "";
            Text2 = "";
            MiddleText = "";
        }

        public void ChangeConnections(Box box1, Box box2)
        {
            Box1 = box1;
            Box2 = box2;
        }
        public void ChangeText1(string text) { Text1 = text; }
        public void ChangeText2(string text) { Text2 = text; }
        public void ChangeMiddleText(string text) { MiddleText = text; }

        public override void CleanUp(Diagram parent)
        {
            Box1.UnlinkLine(this);
            Box2.UnlinkLine(this);
            parent.Remove(this);
        }
    }
}