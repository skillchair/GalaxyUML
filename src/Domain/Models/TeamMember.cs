namespace GalaxyUML.Models
{
    public class TeamMember
    {
        public Guid IdTeamMember { get; set; }
        public Guid IdTeam { get; private set; }
        public Guid IdMember { get; private set; }
        public IRole Role { get; set; }

        public TeamMember(Guid idMember, IRole role)
        {
            idTeamMember = Guid.NewGuid();
            IdTeam = idTeam;
            IdMember = idMember;
            Role = role;
        }
    }
}