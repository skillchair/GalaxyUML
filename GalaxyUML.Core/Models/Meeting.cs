using GalaxyUML.Core.Models.Commands.MeetingCommands;

namespace GalaxyUML.Core.Models
{
    public class Meeting : IMeetingObservable
    {
        //public Guid IdMeeting { get; private set; }
        public DateTime StartingTime { get; private set; }
        public DateTime EndingTime { get; private set; }
        public Chat Chat { get; private set; }
        public Diagram Board { get; private set; }
        public MeetingParticipant Organizer { get; private set; }
        public List<MeetingParticipant> Participants { get; set; }
        public MeetingParticipant ActiveParticipant { get; private set; }
        public bool IsActive { get; private set; }
        public Guid IdTeam { get; private set; }
        public Guid IdOrganizer { get; private set; }
        public Guid IdChat { get; private set; }
        public Guid IdBoard { get; private set; }

        private List<IMeetingObserver> _observers;

        public Meeting(/*Guid id, */Guid idTeam, Guid idMeeting, Guid idOrganizer, Guid idChat, Guid idBoard, TeamMember organizer)
        {
            //IdMeeting = id;
            StartingTime = DateTime.Now;
            EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
            IdChat = idChat;
            Chat = new Chat(idMeeting, this);
            IdBoard = idBoard;
            Board = new Diagram(Constants.Constants.MinPoint, Constants.Constants.MaxPoint, this);
            IdOrganizer = idOrganizer;
            Organizer = new MeetingParticipant(idMeeting, idOrganizer, this, organizer);
            Participants = [Organizer];
            ActiveParticipant = Organizer;
            IsActive = true;
            IdTeam = idTeam;

            _observers = new List<IMeetingObserver>();
        }
        // public Meeting(TeamMember organizer, Diagram board, Chat chat)
        // {
        //     //IdMeeting = Guid.NewGuid();
        //     StartingTime = DateTime.Now;
        //     EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
        //     Chat = chat;
        //     Board = board;
        //     Participants = new List<MeetingParticipant>();
        //     Organizer = new MeetingParticipant(idMeeting, idothis, organizer);
        //     ActiveParticipant = Organizer;
        //     IsActive = true;

        //     _observers = new List<IMeetingObserver>();
        // }

        public Meeting(/*Guid id, */Guid idTeam/*, Guid idMeeting, */,Guid idOrganizer, Guid idChat, Guid idBoard, MeetingParticipant organizer, Diagram board, Chat chat)
        {
            //IdMeeting = id;
            StartingTime = DateTime.Now;
            EndingTime = DateTime.MaxValue;   // ako nije zavrsen teoretski nema kraj
            IdChat = idChat;
            //Chat = new Chat(idMeeting, this);
            Chat = chat;
            IdBoard = idBoard;
            //Board = new Diagram(Constants.Constants.MinPoint, Constants.Constants.MaxPoint, this);
            //IdOrganizer = idOrganizer;
            Board = board;
            //IdMeeting = idMeeting;
            IdOrganizer = idOrganizer;
            Organizer = organizer;//new MeetingParticipant(idMeeting, idOrganizer, this, organizer);
            Participants = [Organizer];
            ActiveParticipant = Organizer;
            IsActive = true;
            IdTeam = idTeam;

            _observers = new List<IMeetingObserver>();
        }

        public void AddParticipant(Guid idMeeting, Guid idParticipant, TeamMember newParticipant)
        {
            var memberInAList = Participants.FirstOrDefault(p => p.TeamMember == newParticipant);
            if (memberInAList != null)
                throw new Exception("User already in a meeting.");

            Participants.Add(new MeetingParticipant(idMeeting, idParticipant, this, newParticipant));
        }

        public void RemoveParticipant(MeetingParticipant participant)
        {
            var participantInAList = Participants.FirstOrDefault(participant);
            if (participantInAList == null)
                throw new Exception("Participant not found.");

            //if (ActiveParticipant.IdMeetingParticipant == participant.IdMeetingParticipant)
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