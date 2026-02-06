namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public abstract class IMeetingCommand
    {
        public Meeting Meeting { get; private set; }

        public IMeetingCommand(Meeting meeting)
        {
            Meeting = meeting;
        }

        public abstract void Execute(MeetingEventType eventType);
    }
}