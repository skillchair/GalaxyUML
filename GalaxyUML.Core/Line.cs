using System.Drawing;
using System.Security.Authentication.ExtendedProtection;

namespace GalaxyUML.Core
{
    public class Line : IDrawable      // PRETPOSTAVLJAMO DA JE SMER UVEK OD CLASS1 KA CLASS2 DA NE BISMO DODATNO PAMTILI SMER!
    {
        public Box Box1 { get; private set; }
        public Box Box2 { get; private set; }
        public string Text1 { get; private set; }
        public string Text2 { get; private set; }
        public string MiddleText { get; private set; }

        public Line(Point startingPoint, Point endingPoint, Box box1, Box box2, string text1, string text2, string middleText)
            : base(startingPoint, endingPoint)
        {
            base.Type = DrawableType.Line;
            Box1 = box1;
            Box2 = box2;
            Text1 = text1;
            Text2 = text2;
            MiddleText = middleText;
        }

        public void ChangeConnections(Box box1, Box box2)
        {
            Box1 = box1;
            Box2 = box2;
        }
        public void ChangeText1(string text) { Text1 = text; }
        public void ChangeText2(string text) { Text2 = text; }
        public void ChangeMiddleText(string text) { MiddleText = text; }

        override public void RemoveSelf()
        {
            Box1.RemoveSelf();
            Box2.RemoveSelf();
            // nema potrebe null-irati; kad se obrise nece postojati, bitno da se obaveste box-ovi o tome!
        }
    }
}