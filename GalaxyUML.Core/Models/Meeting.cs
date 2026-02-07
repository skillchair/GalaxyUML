using GalaxyUML.Core.Models.Commands.MeetingCommands;

namespace GalaxyUML.Core.Models
{
    public class Meeting : IMeetingObservable
    {
        public Guid IdMeeting { get; private set; }
        public DateTime StartingTime { get; private set; }
        public DateTime EndingTime { get; private set; }
        public Chat Chat { get; private set; }
        public Diagram Board { get; private set; }
        public MeetingParticipant Organizer { get; private set; }
        public List<MeetingParticipant> Participants { get; set; }
        public MeetingParticipant ActiveParticipant { get; private set; }
        public bool IsActive { get; private set; }

        private List<IMeetingObserver> _observers;

        public Meeting(TeamMember organizer)
        {
            IdMeeting = Guid.NewGuid();
            StartingTime = DateTime.Now;
            EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
            Chat = new Chat(this);
            Board = new Diagram(this);
            Organizer = new MeetingParticipant(this, organizer);
            Participants = [Organizer];
            ActiveParticipant = Organizer;
            IsActive = true;

            _observers = new List<IMeetingObserver>();
        }
        public Meeting(TeamMember organizer, Diagram board, Chat chat)
        {
            IdMeeting = Guid.NewGuid();
            StartingTime = DateTime.Now;
            EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
            Chat = chat;
            Board = board;
            Participants = new List<MeetingParticipant>();
            Organizer = new MeetingParticipant(this, organizer);
            ActiveParticipant = Organizer;
            IsActive = true;

            _observers = new List<IMeetingObserver>();
        }

        public void AddParticipant(TeamMember newParticipant)
        {
            var memberInAList = Participants.FirstOrDefault(p => p.TeamMember.IdTeamMember == newParticipant.IdTeamMember);
            if (memberInAList != null)
                throw new Exception("User already in a meeting.");

            Participants.Add(new MeetingParticipant(this, newParticipant));
        }

        public void RemoveParticipant(MeetingParticipant participant)
        {
            var participantInAList = Participants.FirstOrDefault(p => p.IdMeetingParticipant == participant.IdMeetingParticipant);
            if (participantInAList == null)
                throw new Exception("Participant not found.");

            if (ActiveParticipant.IdMeetingParticipant == participant.IdMeetingParticipant)
                TakeControl();

            Participants.Remove(participantInAList);
        }

        public void GrantControl(MeetingParticipant participant) { ActiveParticipant = participant; }
        public void TakeControl() { ActiveParticipant = Organizer; }
        public void EndMeeting()
        {
            EndingTime = DateTime.Now;
            IsActive = false;
        }

        public void Attach(IMeetingObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IMeetingObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(MeetingEventType eventType, IMeetingCommand command)
        {
            foreach (var observer in _observers)
                observer.Update(eventType, command);
        }
    }
}