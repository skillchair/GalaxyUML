using System.Text.Json.Serialization;

namespace GalaxyUML.Core.Models
{
    public class MeetingParticipant
    {
        //public Guid IdMeetingParticipant { get; private set; }
        public Meeting Meeting { get; private set; }
        public TeamMember TeamMember { get; private set; }
        public Guid IdMeeting { get; private set; }
        public Guid IdParticipant { get; private set; }

        [JsonConstructor] // Kažeš JSON-u: "Koristi BAŠ ovaj konstruktor"
        public MeetingParticipant(Guid idMeeting, Guid idParticipant, Meeting meeting, TeamMember participant)
        {
            //IdMeetingParticipant = Guid.NewGuid();
            IdMeeting = idMeeting;
            IdParticipant = idParticipant;
            Meeting = meeting;
            TeamMember = participant;
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