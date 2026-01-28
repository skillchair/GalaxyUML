namespace GalaxyUML.Core
{
    public class MeetingParticipant: IMeetingObserver
    {
        public Guid IdMeetingParticipant { get; private set; }
        public Meeting Meeting { get; private set; }
        public User TeamMember { get; private set; }

        public MeetingParticipant(Meeting meeting, User teamMember)
        {
            IdMeetingParticipant = Guid.NewGuid();
            Meeting = meeting;
            TeamMember = teamMember;
        }

        public void ClearEntry()
        {
            Meeting.RemoveParticipant(TeamMember);
            TeamMember.LeaveMeeting();
        }

        public void Update(MeetingEventType meetingEvent)
        {
            throw new NotImplementedException();
        }
    }
}