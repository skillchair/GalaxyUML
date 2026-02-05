namespace GalaxyUML.Core.Models
{
    public class TeamMember//: ITeamObserver<TeamMember>
    {
        public Guid IdTeamMember { get; set; }
        public Team Team { get; private set; }
        public User Member { get; private set; }
        public RoleEnum Role { get; set; }

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
        
        public void Update(TeamEvent<User> teamEvent, ITeamCommand command)
        {
            // Implementation of the observer update method
            // This would be called when team events occur
        }
    }
}