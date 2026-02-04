namespace GalaxyUML.Core.Models
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

        public void RequestControl()
        {
            // nisam siguran kako ovo; da li da se stavljaju svi u red cekanja i da se posto ima ref. na meeting
            // radi meeting.addToWaitingList(this) il kako vec. nek ostane zasad ovako
        }

        public void ClearEntry()
        {
            Meeting.RemoveParticipant(TeamMember);
            TeamMember.LeaveMeeting();
        }

        public override void Update(MeetingEventType meetingEvent)
        {
            throw new NotImplementedException(); 
        }
    }
}