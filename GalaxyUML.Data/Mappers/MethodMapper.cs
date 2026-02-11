using MethodEntity = GalaxyUML.Data.Entities.MethodEntity;
using Method = GalaxyUML.Core.Models.Method;
using ClassBox = GalaxyUML.Core.Models.ClassBox;
using DiagramEntity = GalaxyUML.Data.Entities.DiagramEntity;
using TeamEntity = GalaxyUML.Data.Entities.TeamEntity;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers
{
    class MethodMapper
    {
        public static Method ToModel(MethodEntity entity)
        {
            var classBox = DrawableMapper.ToModel(entity.ClassBox) as ClassBox
                ?? throw new InvalidCastException("entity.ClassBox is not a ClassBox at runtime.");
            return new Method(entity.IdClassBox, classBox, entity.Content);
        }

        public static MethodEntity ToEntity(Method model/*, DiagramEntity parent, TeamEntity team*/)
        {
            var classBox = DrawableMapper.ToEntity(model.ClassBox/*, parent, team*/) as ClassBoxEntity
                ?? throw new InvalidCastException("model.ClassBox is not a ClassBox at runtime.");
            return new MethodEntity
            {
                //Id = model.IdMethod,
                Content = model.Content,
                IdClassBox = model.IdClassBox,
                ClassBox = classBox
            };
        }
    }
}