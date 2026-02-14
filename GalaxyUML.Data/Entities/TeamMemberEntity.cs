using System.ComponentModel.DataAnnotations;
using GalaxyUML.Core.Models; // for RoleEnum; or duplicate enum here if želite čistu separaciju

namespace GalaxyUML.Data.Entities;
public class TeamMemberEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public Guid TeamId { get; set; }
    public TeamEntity Team { get; set; } = null!;
    [Required] public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    [Required] public RoleEnum Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MeetingParticipantEntity> Meetings { get; set; } = new List<MeetingParticipantEntity>();
}
