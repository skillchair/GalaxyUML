using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities;
public class MeetingParticipantEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public Guid MeetingId { get; set; }
    public MeetingEntity Meeting { get; set; } = null!;
    [Required] public Guid TeamMemberId { get; set; }
    public TeamMemberEntity TeamMember { get; set; } = null!;
    public bool CanDraw { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
