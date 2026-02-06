namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public class MeetingEndedCommand : IMeetingCommand
    {
        public List<MeetingParticipant> Participants { get; private set; }
        public MeetingEndedCommand(Meeting meeting, List<MeetingParticipant> participants) : base(meeting)
        {
            Participants = participants;
        }

        public override void Execute(MeetingEventType eventType)
        {
            if (eventType != MeetingEventType.MeetingEnded)
                throw new Exception("Invalid event. Expected MeetingEventType.MeetingEndedCommand.");
            foreach (var participant in Participants)
                Meeting.RemoveParticipant(participant);
        }
    }
}