using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyUML.Data.Entities
{
    class ChatEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdMeeting { get; set; }
        public MeetingEntity Meeting { get; set; } = null!;
        
        public virtual ICollection<MessageEntity> Messages { get; set; } = null!;
    }
}