namespace GalaxyUML.Models
{
    public class MeetingParticipant
    {
        public Guid IdMeetingParticipant { get; private set; }
        public Guid IdMeeting { get; set; }
        public Guid IdTeamMember { get; set; }

        public MeetingParticipant(Guid idMeeting, Guid idTeamMember)
        {
            IdMeetingParticipant = Guid.NewGuid();
            IdMeeting = idMeeting;
            IdTeamMember = idTeamMember;
        }
    }
}