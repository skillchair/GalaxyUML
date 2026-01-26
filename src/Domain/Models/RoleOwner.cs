namespace GalaxyUML.Models
{
    public class RoleOwner: RoleOrganizer
    {
        public RoleOwner()
        {
            base.role = RoleEnum.Owner;
        }

        public GetTeamResult DeleteTeam(Guid idTeam)
        {
            return 0;
        }
    }
}