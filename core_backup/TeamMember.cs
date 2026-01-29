namespace GalaxyUML.Core
{
    public class TeamMember: ITeamObserver
    {
        public Guid IdTeamMember { get; set; }
        public Team Team { get; private set; }
        public User Member { get; private set; }
        public IRole Role { get; set; }

        public TeamMember(Team team, User member, IRole role)
        {
            IdTeamMember = Guid.NewGuid();
            Team = team;
            Member = member;
            Role = role;
        }

        public void ClearEntry()
        {
            Team.RemoveMember(Member);
            Member.LeaveTeam(Team);
        }

        public void ChangeRole(IRole newRole) { Role = newRole; }
    }
}