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
        public Guid ActiveMemberId { get; set; }
        public User Organizer { get; set; }
        public bool IsActive { get; private set; }

        public Meeting(Board board, Chat chat)
        {
            IdMeeting = Guid.NewGuid();
            StartingTime = DateTime.Now();
            EndingTime = DateTime.MaxValue();   // ako nije zavrsen teoretski nema kraj
            Participants = new List<>();
            Board = board;
            Chat = chat;
            IsActive = true;
        }

        public void NotifyAllMembers(List<int> membersIds)
        {
            foreach (int memberId in membersIds)
            {
                // do ...
                // treba da se salju promene o tabli i chat-u
                // to cemo kasnije
            }
        }

        public void RemoveParticipant(Guid idParticipant)
        {
            var participant = Participants.FirstOrDefault(p => p.IdMeetingParticipant == idParticipant);
            if (participant == null)
                throw new Exception("Participant not found.");

            Participants.Remove(participant);
        }

        public void ReleaseBoard()
        {
            ActiveMember = Organizer.IdUser;
        }

        public void EndMeeting()
        {
            IsActive = false;
        }
    }
}