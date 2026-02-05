namespace GalaxyUML.Core.Models
{
    public class Meeting//: IMeetingObservable
    {
        public Guid IdMeeting { get; private set; }
        public DateTime StartingTime { get; private set; }
        public DateTime EndingTime { get; private set; }
        public Chat Chat { get; private set; }
        public Diagram Board { get; private set; }
        public List<MeetingParticipant> Participants { get; set; }
        public TeamMember Organizer { get; private set; }
        public TeamMember ActiveParticipant { get; private set; }
        public bool IsActive { get; private set; }

        public Meeting(TeamMember organizer)
        {
            IdMeeting = Guid.NewGuid();
            StartingTime = DateTime.Now;
            EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
            Chat = new Chat(this);
            Board = new Diagram(this);
            Participants = new List<MeetingParticipant>();
            Organizer = organizer;
            ActiveParticipant = organizer;
            IsActive = true;
        }
        public Meeting(TeamMember organizer, Diagram board, Chat chat)
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

        public void AddParticipant(TeamMember member)
        {
            var memberInAList = Participants.FirstOrDefault(p => p.IdMeetingParticipant == member.IdTeamMember);
            if (memberInAList != null)
                throw new Exception("User already in a meeting.");

            Participants.Add(new MeetingParticipant(this, member));
        }

        // ne valja
        public void RemoveParticipant(MeetingParticipant participant)
        {
            // var participantInAList = Participants.FirstOrDefault(p => p.IdMeetingParticipant == participant.IdMeetingParticipant);
            // if (participantInAList == null)
            //     throw new Exception("Participant not found.");

            // foreach (MeetingParticipant p in Participants)
            //     p.ClearEntry();

            // Participants.Remove(participantInAList);
        }

        // preimenovano u grantcontrol da bude isto kao u dokumentu
        public void GrantControl(TeamMember participant) { ActiveParticipant = participant; }
        public void ReleaseBoard() { ActiveParticipant = Organizer; }
        public void EndMeeting()
        {
            EndingTime = DateTime.Now;
            IsActive = false;
        }

        // void IMeetingObservable.Attach(IMeetingObserver meetingObserver)
        // {
        //     throw new NotImplementedException();
            
        // }

        // void IMeetingObservable.Detach(IMeetingObserver meetingObserver)
        // {
        //     throw new NotImplementedException();
        // }

        public void Notify()
        {
            throw new NotImplementedException();
        }
    }
}