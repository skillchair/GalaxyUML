using Attribute = GalaxyUML.Core.Models.Attribute;
using AttributeEntity = GalaxyUML.Data.Entities.AttributeEntity;

namespace GalaxyUML.Data.Mappers
{
    static class AttributeMapper
    {
        public static AttributeEntity ToEntity(Attribute model)
        {
            if (model == null) return null;
            throw new System.NotImplementedException();
        }
    }
}