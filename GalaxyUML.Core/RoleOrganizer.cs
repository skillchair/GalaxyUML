namespace GalaxyUML.Core
{
    public class RoleOrganizer : IRole
    {
        private Meeting? Meeting { get; set; }

        public RoleOrganizer()
        {
            base.Role = RoleEnum.Organizer;
        }

        public Meeting OrganizeMeeting(User owner)
        {
            if (Meeting != null)
                throw new Exception("You already organized a meeting.");

            return Meeting = new Meeting(owner, new Board(), new Chat());
        }

        public void GiveControl(User participant)
        {
            if (Meeting != null)
                Meeting.GiveControl(participant);
        }

        public void TakeControl()
        {
            if (Meeting != null)
                Meeting.ReleaseBoard();
        }

        public void KickParticipant(User participant)
        {
            if (Meeting != null)
            {
                var meetingParticipant = Meeting.Participants.FirstOrDefault(p => p.TeamMember == participant);
                if (meetingParticipant == null)
                    throw new Exception("Participant not in the meeting.");

                Meeting.Participants.Remove(meetingParticipant);
            }
        }

        public void EndMeeting()
        {
            if (Meeting == null)
                throw new Exception("You did not organize a meeting.");
            
            Meeting.EndMeeting();
            Meeting = null;
        }
    }
}