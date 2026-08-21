using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities;
public class MeetingEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public Guid TeamId { get; set; }
    public TeamEntity Team { get; set; } = null!;
    [Required] public Guid OrganizedById { get; set; }
    public TeamMemberEntity OrganizedBy { get; set; } = null!;
    public DateTime StartingTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndingTime { get; set; }

    [Required] public Guid BoardId { get; set; }
    public DiagramElementEntity Board { get; set; } = null!;
    [Required] public Guid ChatId { get; set; }
    public ChatEntity Chat { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public ICollection<MeetingParticipantEntity> Participants { get; set; } = new List<MeetingParticipantEntity>();
}
