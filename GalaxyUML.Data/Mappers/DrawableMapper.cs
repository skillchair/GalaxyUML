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
                        entity.StartingPoint,
                        entity.EndingPoint,
                        MeetingMapper.ToModel(entity.Meeting),
                        ((BoxEntity)entity).Lines?.Select(l => (Line)ToModel(l)).ToList() ?? new List<Line>()
                    ),

                ObjectType.ClassBox =>
                    new ClassBox(
                        entity.StartingPoint,
                        entity.EndingPoint,
                        MeetingMapper.ToModel(entity.Meeting),
                        ((ClassBoxEntity)entity).Lines?.Select(l => (Line)ToModel(l)).ToList() ?? new List<Line>()
                    ),

                ObjectType.Text =>
                    new Text(
                        entity.StartingPoint,
                        entity.EndingPoint,
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
                        entity.StartingPoint,
                        entity.EndingPoint,
                        MeetingMapper.ToModel(entity.Meeting),
                        (Box)ToModel(((LineEntity)entity).Box1),
                        (Box)ToModel(((LineEntity)entity).Box2)
                    ),

                _ => throw new InvalidOperationException($"Unknown drawable type: {entity.Type}")
            };
        }

        public static DrawableEntity ToEntity(IDrawable model, DiagramEntity parent, TeamEntity teamEntity)
        {
            switch (model.Type)
            {
                case ObjectType.Box:
                    {
                        var boxModel = (Box)model;

                        var lineEntities = boxModel.Lines?
                            .Select(l => (LineEntity)ToEntity(l, parent, teamEntity))
                            .ToList() ?? new List<LineEntity>();

                        return new BoxEntity
                        {
                            Id = boxModel.IdDiagram,
                            Type = boxModel.Type,
                            StartingPoint = boxModel.StartingPoint,
                            EndingPoint = boxModel.EndingPoint,
                            IdMeeting = boxModel.Meeting.IdMeeting,
                            Meeting = MeetingMapper.ToEntity(boxModel.Meeting, teamEntity),
                            IdParent = parent.Id,
                            Parent = parent,
                            Objects = null,
                            Lines = lineEntities
                        };
                    }

                case ObjectType.ClassBox:
                    {
                        var classBoxModel = (ClassBox)model;

                        var lineEntities = classBoxModel.Lines?
                            .Select(l => (LineEntity)ToEntity(l, parent, teamEntity))
                            .ToList() ?? new List<LineEntity>();

                        return new ClassBoxEntity
                        {
                            Id = classBoxModel.IdDiagram,
                            Type = classBoxModel.Type,
                            StartingPoint = classBoxModel.StartingPoint,
                            EndingPoint = classBoxModel.EndingPoint,
                            IdMeeting = classBoxModel.Meeting.IdMeeting,
                            Meeting = MeetingMapper.ToEntity(classBoxModel.Meeting, teamEntity),
                            IdParent = parent.Id,
                            Parent = parent,
                            Objects = null,
                            Lines = lineEntities
                        };
                    }

                case ObjectType.Text:
                    var textModel = (Text)model;
                    return new TextEntity
                    {
                        Id = textModel.IdDiagram,
                        Type = textModel.Type,
                        StartingPoint = textModel.StartingPoint,
                        EndingPoint = textModel.EndingPoint,
                        IdMeeting = textModel.Meeting.IdMeeting,
                        Meeting = MeetingMapper.ToEntity(textModel.Meeting, teamEntity),
                        IdParent = parent.Id,
                        Parent = parent,
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
                        Id = lineModel.IdDiagram,
                        Type = lineModel.Type,
                        StartingPoint = lineModel.StartingPoint,
                        EndingPoint = lineModel.EndingPoint,
                        IdMeeting = lineModel.Meeting.IdMeeting,
                        Meeting = MeetingMapper.ToEntity(lineModel.Meeting, teamEntity),
                        IdParent = parent.Id,
                        Parent = parent,
                        Objects = null,
                        Box1 = (BoxEntity)ToEntity(lineModel.Box1, parent, teamEntity),
                        Box2 = (BoxEntity)ToEntity(lineModel.Box2, parent, teamEntity)
                    };

                default:
                    throw new InvalidOperationException($"Unknown drawable type: {model.Type}");
            }
        }
    }
}