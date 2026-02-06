namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public class ObjectAddedCommand : IMeetingCommand
    {
        public Diagram Parent { get; private set; }
        public IDiagram Obj { get; private set; }

        public ObjectAddedCommand(Meeting meeting, Diagram parent, IDiagram obj) : base(meeting)
        {
            Parent = parent;
            Obj = obj;
        }

        public override void Execute(MeetingEventType eventType)
        {
            if (eventType != MeetingEventType.ObjectAdded)
                throw new Exception("Invalid event. Expected MeetingEventType.ObjectAdded.");

            Parent.Add(Obj);
        }
    }
}