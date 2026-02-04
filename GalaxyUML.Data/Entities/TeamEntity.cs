using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data
{
    class TeamEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(6)]
        public string Code { get; set; } = null!;

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        [ForeignKey("OwnerId")]
        [MaxLength(7)]
        public UserEntity Owner { get; set; } = null!;

        public ICollection<TeamMemberEntity> Members { get; set; } = new List<TeamMemberEntity>();
    }

}