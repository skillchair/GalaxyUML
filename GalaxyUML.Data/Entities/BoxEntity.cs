using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    class BoxEntity
    {
        [Key]
        public Guid IdDrawable { get; set; }    // svi drawable objekti se identifikuju pomocu primarnog kljuca IdDrawable

        public virtual ICollection<LineEntity> Lines { get; set; } = new List<LineEntity>();
    }
}