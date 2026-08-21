using System.ComponentModel.DataAnnotations;

namespace GalaxyUML.Data.Entities;
public class UserEntity
{
    [Key] public Guid Id { get; set; }
    [Required, MaxLength(80)]  public string FirstName { get; set; } = null!;
    [Required, MaxLength(80)]  public string LastName { get; set; } = null!;
    [Required, MaxLength(80)]  public string Username { get; set; } = null!;
    [Required, MaxLength(200)] public string Email { get; set; } = null!;
    [Required, MaxLength(200)] public string Password { get; set; } = null!;
}
