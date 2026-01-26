namespace GalaxyUML.Models
{
    public class RoleOrganizer: IRole
    {
        public Meeting Meeting { get; set; }

        public RoleOrganizer(RoleEnum role, Meeting meeting)
        {
            IRole(role);

            Meeting = meeting;
        }

        public Meeting OrganizeMeeting()
        {
            //
        }

        public GivePermissionResult GivePermission(Guid idParticipant)
        {
            //
            return 0;
        }

        public TakePermissionResult TakePermission(Guid idParticipant)
        {
            return 0;
        }

        public GetParticipantResult KickParticipant(Guid idParticipant)
        {
            return 0;
        }

        public GetMeetingResult EndMeeting()
        {
            return 0;
        }
    }
}