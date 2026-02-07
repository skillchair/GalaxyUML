using Diagram = GalaxyUML.Core.Models.Diagram;
using IDiagram = GalaxyUML.Core.Models.IDiagram;
using IDrawable = GalaxyUML.Core.Models.IDrawable;
using DiagramEntity = GalaxyUML.Data.Entities.DiagramEntity;
using DrawableEntity = GalaxyUML.Data.Entities.DrawableEntity;
using ObjectType = GalaxyUML.Core.Models.ObjectType;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers
{
    static class DiagramMapper
    {
        public static Diagram ToModel(DiagramEntity entity)
        {
            List<IDiagram> objs = new List<IDiagram>();
            if (entity.Objects != null)
                foreach (var o in entity.Objects)
                    switch (o.Type)
                    {
                        case ObjectType.Diagram:
                            objs.Add(ToModel(o));
                            break;

                        default:
                            objs.Add(DrawableMapper.ToModel((DrawableEntity)o));
                            break;
                    }
            return new Diagram(
                entity.StartingPoint,
                entity.EndingPoint,
                MeetingMapper.ToModel(entity.Meeting),
                objs
            );
        }

        public static DiagramEntity ToEntity(Diagram model, DiagramEntity? parent, TeamEntity teamEntity)
        {
            DiagramEntity diagramEntity = new DiagramEntity()
            {
                Id = model.IdDiagram,
                Type = model.Type,
                StartingPoint = model.StartingPoint,
                EndingPoint = model.EndingPoint,
                IdMeeting = model.Meeting.IdMeeting,
                Meeting = MeetingMapper.ToEntity(model.Meeting, teamEntity),
                IdParent = parent?.Id,
                Parent = parent,
                Objects = new List<DiagramEntity>() // prazno za sad
            };

            List<DiagramEntity> objs = new List<DiagramEntity>();
            if (model.Objs != null)
                foreach (var o in model.Objs)
                    switch (o.Type)
                    {
                        case ObjectType.Diagram:
                            objs.Add(ToEntity((Diagram)o, diagramEntity, teamEntity));
                            break;

                        default:
                            objs.Add(DrawableMapper.ToEntity((IDrawable)o, diagramEntity, teamEntity));
                            break;
                    }

            return diagramEntity;
        }
    }
}