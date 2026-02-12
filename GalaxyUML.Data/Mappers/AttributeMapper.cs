using AttributeEntity = GalaxyUML.Data.Entities.AttributeEntity;
using Attribute = GalaxyUML.Core.Models.Attribute;
using ClassBox = GalaxyUML.Core.Models.ClassBox;
using DiagramEntity = GalaxyUML.Data.Entities.DiagramEntity;
using TeamEntity = GalaxyUML.Data.Entities.TeamEntity;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers
{
    class AttributeMapper
    {
        public static Attribute ToModel(AttributeEntity entity)
        {
            var classBox = DrawableMapper.ToModel(entity.ClassBox) as ClassBox
                ?? throw new InvalidCastException("entity.ClassBox is not a ClassBox at runtime.");
            return new Attribute(entity.IdClassBox, classBox, entity.Content);
        }

        public static AttributeEntity ToEntity(Attribute model)//, DiagramEntity parent/*, TeamEntity team*/)
        {
            // var classBox = DrawableMapper.ToEntity(model.ClassBox/*, parent/*, team*/) as ClassBoxEntity
            //     ?? throw new InvalidCastException("model.ClassBox is not a ClassBox at runtime.");
            return new AttributeEntity
            {
                //Id = model.IdAttribute,
                Content = model.Content,
                IdClassBox = model.IdClassBox,
                //ClassBox = classBox
            };
        }
    }
}