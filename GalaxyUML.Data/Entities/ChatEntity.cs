using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities
{
    public class ChatEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdMeeting { get; set; }
        public MeetingEntity Meeting { get; set; } = null!;
        
        public virtual ICollection<MessageEntity>? Messages { get; set; }
    }
}