namespace GalaxyUML.Core
{
    public class Meeting
    {
        public Guid IdMeeting { get; private set; }
        public DateTime StartingTime { get; private set; }
        public DateTime EndingTime { get; private set; }
        public Chat Chat { get; private set; }
        public Board Board { get; private set; }
        public List<MeetingParticipant> Participants { get; set; }
        public User Organizer { get; private set; }
        public User ActiveParticipant { get; private set; }
        public bool IsActive { get; private set; }

        public Meeting(User organizer, Board board, Chat chat)
        {
            IdMeeting = Guid.NewGuid();
            StartingTime = DateTime.Now;
            EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
            Chat = chat;
            Board = board;
            Participants = new List<MeetingParticipant>();
            Organizer = organizer;
            ActiveParticipant = organizer;
            IsActive = true;
        }

        public void AddParticipant(User member)
        {
            var memberInAList = Participants.FirstOrDefault(p => p.IdMeetingParticipant == member.IdUser);
            if (memberInAList != null)
                throw new Exception("User already in a meeting.");

            Participants.Add(new MeetingParticipant(this, member));
        }

        public void RemoveParticipant(User participant)
        {
            var participantInAList = Participants.FirstOrDefault(p => p.IdMeetingParticipant == participant.IdUser);
            if (participantInAList == null)
                throw new Exception("Participant not found.");

            foreach (MeetingParticipant p in Participants)
                p.ClearEntry();

            Participants.Remove(participantInAList);
        }

        public void GiveControl(User participant) { ActiveParticipant = participant; }
        public void ReleaseBoard() { ActiveParticipant = Organizer; }
        public void EndMeeting()
        {
            EndingTime = DateTime.Now;
            IsActive = false;
        }
    }
}