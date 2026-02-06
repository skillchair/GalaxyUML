namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public class ObjectRemovedCommand : IMeetingCommand
    {
        public Diagram Parent { get; private set; }
        public IDiagram Obj { get; private set; }

        public ObjectRemovedCommand(Meeting meeting, Diagram parent, IDiagram obj) : base(meeting)
        {
            Parent = parent;
            Obj = obj;
        }

        public override void Execute(MeetingEventType eventType)
        {
            if (eventType != MeetingEventType.ObjectRemoved)
                throw new Exception("Invalid event. Expected MeetingEventType.ObjectRemoved.");

            Parent.Remove(Obj);
        }
    }
}