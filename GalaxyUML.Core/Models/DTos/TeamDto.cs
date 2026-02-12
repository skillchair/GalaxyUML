// GalaxyUML.Core.Models.DTOs/CreateTeamDto.cs
namespace GalaxyUML.Core.Models.DTOs
{
    public class TeamDto
    {
        public string TeamName { get; set; } = null!;
        public Guid IdTeamOwner { get; set; }
    }
}