using System.Drawing;
using System.Security.Authentication.ExtendedProtection;

namespace GalaxyUML.Core
{
    public class Line : IDrawable      // PRETPOSTAVLJAMO DA JE SMER UVEK OD CLASS1 KA CLASS2 DA NE BISMO DODATNO PAMTILI SMER!
    {
        public ClassBox Class1 { get; private set; }
        public ClassBox Class2 { get; private set; }
        public string Text1 { get; private set; }
        public string Text2 { get; private set; }
        public string MiddleText { get; private set; }

        public Line(Point startingPoint, Point endingPoint, ClassBox class1, ClassBox class2, string text1, string text2, string middleText)
            : base(startingPoint, endingPoint)
        {
            base.Type = DrawableType.Line;
            Class1 = class1;
            Class2 = class2;
            Text1 = text1;
            Text2 = text2;
            MiddleText = middleText;
        }

        public void ChangeConnections(ClassBox class1, ClassBox class2)
        {
            Class1 = class1;
            Class2 = class2;
        }
        public void ChangeText1(string text) { Text1 = text; }
        public void ChangeText2(string text) { Text2 = text; }
        public void ChangeMiddleText(string text) { MiddleText = text; }
    }
}