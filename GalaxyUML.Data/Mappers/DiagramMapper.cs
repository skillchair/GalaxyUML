using System.Drawing;
using System.Reflection;
using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class DiagramMapper
{
    // --- helpers to reach private children/lines ---
    static List<IDiagram> ChildrenOf(Diagram d) =>
        (List<IDiagram>)(typeof(Diagram).GetField("_children", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(d)!);

    static List<Line> LinesOf(Box b) =>
        (List<Line>)(typeof(Box).GetField("_lines", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(b)!);

    // --- domain -> entity ---
    public static DiagramElementEntity ToEntity(IDiagram d)
    {
        return d switch
        {
            Diagram dg => MapDiagram(dg),
            Text t => MapText(t),
            ClassBox cb => MapClassBox(cb),
            Box b => MapBox(b),
            Line l => MapLine(l),
            _ => throw new NotSupportedException($"Unknown diagram type {d.GetType().Name}")
        };
    }

    static DiagramEntity MapDiagram(Diagram d)
    {
        var ent = new DiagramEntity
        {
            Id = d.Id,
            ObjectType = ObjectType.Diagram,
            X1 = d.StartingPoint.X, Y1 = d.StartingPoint.Y, X2 = d.EndingPoint.X, Y2 = d.EndingPoint.Y
        };
        ent.Children = ChildrenOf(d).Select(ToEntity).ToList();
        return ent;
    }

    static TextEntity MapText(Text t) => new()
    {
        Id = t.Id,
        ObjectType = ObjectType.Text,
        X1 = t.StartingPoint.X, Y1 = t.StartingPoint.Y, X2 = t.EndingPoint.X, Y2 = t.EndingPoint.Y,
        Content = t.Content, FontSize = t.FontSize, Color = t.Color, Format = t.Format
    };

    static BoxEntity MapBox(Box b) => new()
    {
        Id = b.Id,
        ObjectType = ObjectType.Box,
        X1 = b.StartingPoint.X, Y1 = b.StartingPoint.Y, X2 = b.EndingPoint.X, Y2 = b.EndingPoint.Y,
        Outgoing = LinesOf(b).Select(MapLine).ToList(), // stored as outgoing for both ends
        Incoming = new List<LineEntity>()
    };

    static ClassBoxEntity MapClassBox(ClassBox c)
    {
        var ent = new ClassBoxEntity
        {
            Id = c.Id,
            ObjectType = ObjectType.ClassBox,
            X1 = c.StartingPoint.X, Y1 = c.StartingPoint.Y, X2 = c.EndingPoint.X, Y2 = c.EndingPoint.Y,
            Attributes = c.Attributes.Select(a => new ClassAttributeEntity { Id = Guid.NewGuid(), Name = a }).ToList(),
            Methods = c.Methods.Select(m => new ClassMethodEntity { Id = Guid.NewGuid(), Signature = m }).ToList()
        };
        return ent;
    }

    static LineEntity MapLine(Line l) => new()
    {
        Id = l.Id,
        ObjectType = ObjectType.Line,
        X1 = l.StartingPoint.X, Y1 = l.StartingPoint.Y, X2 = l.EndingPoint.X, Y2 = l.EndingPoint.Y,
        StartBoxId = l.BoxId,
        EndBoxId = l.BoxId,
        MiddleText = l.MiddleText, Text1 = l.Text1, Text2 = l.Text2
    };

    // --- entity -> domain ---
    public static IDiagram ToDomain(DiagramElementEntity e, Diagram parent)
    {
        return e.ObjectType switch
        {
            ObjectType.Diagram => FromDiagram((DiagramEntity)e, parent),
            ObjectType.Text => FromText((TextEntity)e, parent),
            ObjectType.Box => FromBox((BoxEntity)e, parent),
            ObjectType.ClassBox => FromClassBox((ClassBoxEntity)e, parent),
            ObjectType.Line => FromLine((LineEntity)e, parent),
            _ => throw new NotSupportedException()
        };
    }

    public static Diagram FromDiagram(DiagramEntity e, Diagram parent)
    {
        var d = new Diagram(new Point((int)e.X1, (int)e.Y1), new Point((int)e.X2, (int)e.Y2));
        var children = ChildrenOf(d);
        // two‑pass to resolve boxes before lines
        var childEntities = e.Children.ToList();
        var boxes = new Dictionary<Guid, Box>();

        foreach (var ce in childEntities.Where(c => c.ObjectType is ObjectType.Diagram or ObjectType.Text or ObjectType.Box or ObjectType.ClassBox))
        {
            var child = ToDomain(ce, d);
            children.Add(child);
            if (child is Box bx) boxes[bx.Id] = bx;
        }

        foreach (var ce in childEntities.Where(c => c.ObjectType == ObjectType.Line))
        {
            var le = (LineEntity)ce;
            if (!boxes.TryGetValue(le.StartBoxId, out var start)) continue;
            var line = CreateLine(start, le);
            children.Add(line);
        }

        return d;
    }

    static Text FromText(TextEntity e, Diagram parent) =>
        new Text(new Point((int)e.X1, (int)e.Y1), new Point((int)e.X2, (int)e.Y2), parent)
        {
            // Update via method to keep invariants
        }.Also(t => t.Update(e.Content, e.FontSize, e.Color, e.Format));

    static Box FromBox(BoxEntity e, Diagram parent) =>
        new Box(new Point((int)e.X1, (int)e.Y1), new Point((int)e.X2, (int)e.Y2), parent);

    static ClassBox FromClassBox(ClassBoxEntity e, Diagram parent)
    {
        var cb = new ClassBox(new Point((int)e.X1, (int)e.Y1), new Point((int)e.X2, (int)e.Y2), parent);
        foreach (var a in e.Attributes) cb.AddAttribute(a.Name);
        foreach (var m in e.Methods) cb.AddMethod(m.Signature);
        return cb;
    }

    static Line FromLine(LineEntity e, Diagram parent)
    {
        // Requires a Box; expect caller to create via CreateLine using box dictionary
        throw new InvalidOperationException("Line should be created via CreateLine with a Box reference");
    }

    static Line CreateLine(Box box, LineEntity e)
    {
        var ctor = typeof(Line).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                               null,
                                               new[] { typeof(Box), typeof(string), typeof(string), typeof(string) },
                                               null)!;
        var line = (Line)ctor.Invoke(new object?[] { box, e.MiddleText, e.Text1, e.Text2 });
        // set coordinates if needed (protected setters), using reflection
        typeof(IDiagram).GetProperty("StartingPoint", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                        .SetValue(line, new Point((int)e.X1, (int)e.Y1));
        typeof(IDiagram).GetProperty("EndingPoint", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                        .SetValue(line, new Point((int)e.X2, (int)e.Y2));
        LinesOf(box).Add(line);
        return line;
    }

    // small helper to allow fluent Update call
    static T Also<T>(this T obj, Action<T> act) { act(obj); return obj; }
}
