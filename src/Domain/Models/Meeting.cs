using System.Runtime.InteropServices;

namespace GalaxyUML.Models
{
    public class Meeting
    {
        public Guid IdMeeting { get; private set; }
        public DateTime StartingTime { get; private set; }
        public DateTime EndingTime { get; set; }
        public Chat Chat { get; private set; }
        public Board Board { get; private set; }
        public List<MeetingParticipant> Participants { get; set; }

        public Meeting()
        {
            IdMeeting = Guid.NewGuid();
            StartingTime = DateTime.Now();
            EndingTime = DateTime.MaxValue();   // ako nije zavrsen teoretski nema kraj
        }

        public void NotifyAllMembers(List<int> membersIds)
        {
            foreach (int memberId in membersIds)
            {
                // do ...
            }
        }

        public void RemoveParticipant(Guid idParticipant)
        {
            //
        }
    }
}