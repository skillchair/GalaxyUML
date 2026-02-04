using System;
using System.Drawing;
public abstract class IDiagram
{
    protected Guid idDiagram;
    protected Point startingPoint;
    protected Point endingPoint;
    protected Meeting meeting;

    public abstract void moveDiagram(Point newStartingPoint);
    public abstract void resizeDiagram(Point newEndingPoint);
}