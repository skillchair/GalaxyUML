using IDrawable = GalaxyUML.Core.Models.IDrawable;
using Box = GalaxyUML.Core.Models.Box;
using Text = GalaxyUML.Core.Models.Text;
using Line = GalaxyUML.Core.Models.Line;
using ClassBox = GalaxyUML.Core.Models.ClassBox;
using DiagramEntity = GalaxyUML.Data.Entities.DiagramEntity;
using DrawableEntity = GalaxyUML.Data.Entities.DrawableEntity;
using BoxEntity = GalaxyUML.Data.Entities.BoxEntity;
using TextEntity = GalaxyUML.Data.Entities.TextEntity;
using LineEntity = GalaxyUML.Data.Entities.LineEntity;
using ClassBoxEntity = GalaxyUML.Data.Entities.ClassBoxEntity;
using ObjectType = GalaxyUML.Core.Models.ObjectType;
using GalaxyUML.Data.Entities;
using System.Drawing;

namespace GalaxyUML.Data.Mappers
{
    static class DrawableMapper
    {
        public static IDrawable ToModel(DrawableEntity entity)
        {
            return entity.Type switch
            {
                ObjectType.Box =>
                    new Box(
                        new Point(entity.StartX, entity.StartY),
                        new Point(entity.EndX, entity.EndY),
                        MeetingMapper.ToModel(entity.Meeting),
                        ((BoxEntity)entity).LinesAsStart?.Select(l => (Line)ToModel(l)).ToList() ?? new List<Line>(),
                        ((BoxEntity)entity).LinesAsEnd?.Select(l => (Line)ToModel(l)).ToList() ?? new List<Line>()
                    ),

                ObjectType.ClassBox =>
                    new ClassBox(
                        new Point(entity.StartX, entity.StartY),
                        new Point(entity.EndX, entity.EndY),
                        MeetingMapper.ToModel(entity.Meeting),
                        ((ClassBoxEntity)entity).LinesAsStart?.Select(l => (Line)ToModel(l)).ToList() ?? new List<Line>(),
                        ((ClassBoxEntity)entity).LinesAsEnd?.Select(l => (Line)ToModel(l)).ToList() ?? new List<Line>()
                    ),

                ObjectType.Text =>
                    new Text(
                        new Point(entity.StartX, entity.StartY),
                        new Point(entity.EndX, entity.EndY),
                        MeetingMapper.ToModel(entity.Meeting),
                        ((TextEntity)entity).Content,
                        ((TextEntity)entity).Format,
                        ((TextEntity)entity).Size,
                        ((TextEntity)entity).Color
                    ),

                ObjectType.Line
                or ObjectType.Association
                or ObjectType.DirectedAssociation
                or ObjectType.Aggregation
                or ObjectType.Composition
                or ObjectType.Dependency
                or ObjectType.Generalization
                or ObjectType.Realization =>
                    new Line(
                        new Point(entity.StartX, entity.StartY),
                        new Point(entity.EndX, entity.EndY),
                        MeetingMapper.ToModel(entity.Meeting),
                        (Box)ToModel(((LineEntity)entity).Box1),
                        (Box)ToModel(((LineEntity)entity).Box2)
                    ),

                _ => throw new InvalidOperationException($"Unknown drawable type: {entity.Type}")
            };
        }

        public static DrawableEntity ToEntity(IDrawable model/*, DiagramEntity parent/*, TeamEntity teamEntity*/)
        {
            switch (model.Type)
            {
                case ObjectType.Box:
                    {
                        var boxModel = (Box)model;

                        // var lineStartEntities = boxModel.LinesAsStart?
                        //     .Select(l => (LineEntity)ToEntity(l/*, parent/*, teamEntity*/))
                        //     .ToList() ?? new List<LineEntity>();

                        // var lineEndEntities = boxModel.LinesAsEnd?
                        //     .Select(l => (LineEntity)ToEntity(l/*, parent/*, teamEntity*/))
                        //     .ToList() ?? new List<LineEntity>();

                        return new BoxEntity
                        {
                            //Id = boxModel.IdDiagram,
                            Type = boxModel.Type,
                            StartX = boxModel.StartingPoint.X,
                            StartY = boxModel.StartingPoint.Y,
                            EndX = boxModel.EndingPoint.X,
                            EndY = boxModel.EndingPoint.Y,
                            IdMeeting = boxModel.IdMeeting,
                            //Meeting = MeetingMapper.ToEntity(boxModel.Meeting/*, teamEntity),
                            IdParent = model.IdParent,
                            //Parent = parent,
                            Objects = null,
                            //LinesAsStart = lineStartEntities,
                            //LinesAsEnd = lineEndEntities
                        };
                    }

                case ObjectType.ClassBox:
                    {
                        var classBoxModel = (ClassBox)model;

                        // var lineStartEntities = classBoxModel.LinesAsStart?
                        //     .Select(l => (LineEntity)ToEntity(l/*, parent/*, teamEntity*/))
                        //     .ToList() ?? new List<LineEntity>();

                        // var lineEndEntities = classBoxModel.LinesAsEnd?
                        //     .Select(l => (LineEntity)ToEntity(l/*, parent/*, teamEntity*/))
                        //     .ToList() ?? new List<LineEntity>();

                        return new ClassBoxEntity
                        {
                            //Id = classBoxModel.IdDiagram,
                            Type = classBoxModel.Type,
                            StartX = classBoxModel.StartingPoint.X,
                            StartY = classBoxModel.StartingPoint.Y,
                            EndX = classBoxModel.EndingPoint.X,
                            EndY = classBoxModel.EndingPoint.Y,
                            IdMeeting = classBoxModel.IdMeeting,
                            //Meeting = MeetingMapper.ToEntity(classBoxModel.Meeting/*, teamEntity*/),
                            IdParent = model.IdParent,
                            //IdParent = parent.Id,
                            //Parent = parent,
                            //Objects = null,
                            //LinesAsStart = lineStartEntities,
                            //LinesAsEnd = lineEndEntities
                        };
                    }

                case ObjectType.Text:
                    var textModel = (Text)model;
                    return new TextEntity
                    {
                        //Id = textModel.IdDiagram,
                        Type = textModel.Type,
                        StartX = textModel.StartingPoint.X,
                        StartY = textModel.StartingPoint.Y,
                        EndX = textModel.EndingPoint.X,
                        EndY = textModel.EndingPoint.Y,
                        IdMeeting = textModel.IdMeeting,
                        //Meeting = MeetingMapper.ToEntity(textModel.Meeting/*, teamEntity*/),
                        // IdParent = parent.Id,
                        // Parent = parent,
                        IdParent = model.IdParent,
                        Objects = null,
                        Content = textModel.Content,
                        Format = textModel.Format,
                        Size = textModel.Size,
                        Color = textModel.Color
                    };

                case ObjectType.Line:
                case ObjectType.Association:
                case ObjectType.DirectedAssociation:
                case ObjectType.Aggregation:
                case ObjectType.Composition:
                case ObjectType.Dependency:
                case ObjectType.Generalization:
                case ObjectType.Realization:
                    var lineModel = (Line)model;
                    return new LineEntity
                    {
                        //Id = lineModel.IdDiagram,
                        Type = lineModel.Type,
                        StartX = lineModel.StartingPoint.X,
                        StartY = lineModel.StartingPoint.Y,
                        EndX = lineModel.EndingPoint.X,
                        EndY = lineModel.EndingPoint.Y,
                        IdMeeting = lineModel.IdMeeting,
                        //Meeting = MeetingMapper.ToEntity(lineModel.Meeting/*, teamEntity*/),
                        // IdParent = parent.Id,
                        // Parent = parent,
                        IdParent = model.IdParent,
                        Objects = null,
                        //Box1 = (BoxEntity)ToEntity(lineModel.Box1/*, parent, teamEntity*/),
                        //Box2 = (BoxEntity)ToEntity(lineModel.Box2/*, parent, teamEntity*/)
                    };

                default:
                    throw new InvalidOperationException($"Unknown drawable type: {model.Type}");
            }
        }
    }
}