namespace GalaxyUML.Models
{
    public class RoleOrganizer : IRole
    {
        public Meeting Meeting { get; set; }

        public RoleOrganizer()
        {
            base.role = RoleEnum.Organizer;
        }

        public Meeting OrganizeMeeting()
        {
            if (Meeting == null)
                throw new Exception("You already organized a meeting.");

            return Meeting = new Meeting(new Board(), new Chat());
        }

        public void GiveControl(Guid idParticipant)
        {
            Meeting.ActiveMemberId = idParticipant;
        }

        public void TakeControl()
        {
            Meeting.ReleaseBoard();
        }

        public void KickParticipant(Guid idParticipant)
        {
            var participant = Meeting.Participants.FirstOrDefault(p => p.idParticipant == idParticipant);
            if (participant == null)
                throw new Exception("Participant not in the meeting.");

            Meeting.Participants.Remove(participant);
        }

        public void EndMeeting()
        {
            Meeting.EndMeeting();
        }
    }
}