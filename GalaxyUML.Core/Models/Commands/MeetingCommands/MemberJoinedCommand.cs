namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public class MemberJoinedCommand: IMeetingCommand
    {
        public TeamMember NewParticipant { get; private set; }
        public Meeting Meeting { get; set; }
        public List<MeetingParticipant> Participants { get; private set; }

        public MemberJoinedCommand(TeamMember newParticipant, Meeting meeting, List<MeetingParticipant> participants)
        {
            NewParticipant = newParticipant;
            Meeting = meeting;
            Participants = participants;
        }

        public void Execute()
        {
            Participants.Add(new MeetingParticipant(Meeting, NewParticipant));
        }
    }
}