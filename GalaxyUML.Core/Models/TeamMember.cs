using System.Text.Json.Serialization;

namespace GalaxyUML.Core.Models
{
    public class TeamMember
    {
        //public Guid IdTeamMember { get; private set; }
        public Team Team { get; private set; }
        public Guid IdTeam { get; private set; }
        public User Member { get; private set; }
        public RoleEnum Role { get; private set; }

        [JsonConstructor] // Kažeš JSON-u: "Koristi BAŠ ovaj konstruktor"
        public TeamMember(Guid idTeam,Team team, User member, RoleEnum role)
        {
            //IdTeamMember = Guid.NewGuid();
            IdTeam = idTeam;
            Team = team;
            Member = member;
            Role = role;
        }

        // public void ClearEntry()
        // {
        //     Team.RemoveMember(Member);
        //     //Member.LeaveTeam(Team);
        // }

        public void ChangeRole(RoleEnum newRole) { Role = newRole; }
        public void LeaveTeam(Team team) { Member.LeaveTeam(team); }
    }
}