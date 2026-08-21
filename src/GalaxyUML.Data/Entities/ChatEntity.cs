using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities;
public class ChatEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public Guid MeetingId { get; set; }
    public MeetingEntity Meeting { get; set; } = null!;
    public ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
}

public class MessageEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public Guid ChatId { get; set; }
    public ChatEntity Chat { get; set; } = null!;
    [Required] public Guid SenderId { get; set; }   // UserId
    public UserEntity Sender { get; set; } = null!;
    [Required, MaxLength(2000)] public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
