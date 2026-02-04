using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class BoxEntity
    {
        [Key]
        public Guid IdDrawable { get; set; }    // svi drawable objekti se identifikuju pomocu primarnog kljuca IdDrawable

        public virtual ICollection<LineEntity> Lines { get; set; }
    }
}