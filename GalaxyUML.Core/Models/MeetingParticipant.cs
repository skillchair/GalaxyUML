namespace GalaxyUML.Core.Models
{
    public class MeetingParticipant
    {
        //public Guid IdMeetingParticipant { get; private set; }
        public Meeting Meeting { get; private set; }
        public TeamMember TeamMember { get; private set; }
        public Guid IdMeeting { get; private set; }
        public Guid IdParticipant { get; private set; }

        public MeetingParticipant(Guid idMeeting, Guid idParticipant, Meeting meeting, TeamMember teamMember)
        {
            //IdMeetingParticipant = Guid.NewGuid();
            IdMeeting = idMeeting;
            IdParticipant = idParticipant;
            Meeting = meeting;
            TeamMember = teamMember;
        }

        // public void RequestControl()
        // {
        //     // nisam siguran kako ovo; da li da se stavljaju svi u red cekanja i da se posto ima ref. na meeting
        //     // radi meeting.addToWaitingList(this) il kako vec. nek ostane zasad ovako
        // }

        // // ne valja
        // // public void ClearEntry()
        // // {
        // //     Meeting.RemoveParticipant(this);
        // //     TeamMember.LeaveMeeting();
        // // }
    }
}