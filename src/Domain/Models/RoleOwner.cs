namespace GalaxyUML.Models
{
    public class RoleOwner: RoleOrganizer
    {
        public RoleOwner(RoleEnum role)
        {
            RoleOrganizer(role);
        }

        public GetTeamResult DeleteTeam(Guid idTeam)
        {
            return 0;
        }
    }
}