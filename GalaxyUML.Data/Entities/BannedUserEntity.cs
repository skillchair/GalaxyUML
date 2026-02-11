using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class BannedUserEntity
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
        public Guid IdUser { get; set; }
        public UserEntity User { get; set; } = null!;

        [Required]
        public Guid IdTeam { get; set; }
        public TeamEntity Team { get; set; } = null!;
    }
}