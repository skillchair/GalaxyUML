namespace GalaxyUML.Core
{
    public class RoleOwner: RoleOrganizer
    {
        public Team Team { get; set; }
        public RoleOwner(Team team)
        {
            base.Role = RoleEnum.Owner;
            Team = team;
        }

        public void DeleteTeam()
        {
            foreach (TeamMember member in Team.Members)
                member.ClearEntry();
        }
    }
}