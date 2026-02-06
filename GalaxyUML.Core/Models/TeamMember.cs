namespace GalaxyUML.Core.Models
{
    public class TeamMember
    {
        public Guid IdTeamMember { get; private set; }
        public Team Team { get; private set; }
        public User Member { get; private set; }
        public RoleEnum Role { get; private set; }

        public TeamMember(Team team, User member, RoleEnum role)
        {
            IdTeamMember = Guid.NewGuid();
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