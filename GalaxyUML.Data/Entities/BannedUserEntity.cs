using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities;
public class BannedUserEntity
{
    [Key] public Guid Id { get; set; }
    [Required] public Guid TeamId { get; set; }
    public TeamEntity Team { get; set; } = null!;
    [Required] public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public DateTime BannedAt { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
}
