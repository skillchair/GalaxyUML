namespace GalaxyUML.Core.Models.Commands.MeetingCommands
{
    public class ObjectEdittedCommand : IMeetingCommand
    {
        public List<IDiagram> ParentObjs { get; private set; }
        public IDiagram Obj { get; private set; }

        public ObjectEdittedCommand(Meeting meeting, List<IDiagram> parentObjs, IDiagram obj) : base(meeting)
        {
            ParentObjs = parentObjs;
            Obj = obj;
        }

        // NIJE DOBRO
        public override void Execute(MeetingEventType eventType)
        {
            if (eventType != MeetingEventType.ObjectEditted)
                throw new Exception("Invalid event. Expected MeetingEventType.ObjectAdded.");

            // mozda bolje edit, ne znam; imamo odvojene metode za move i resize. poenta mi je nekako da
            // svaka promena "trigeruje" editted event
            //int index = ParentObjs.FindIndex(o => o.IdDiagram == Obj.IdDiagram);
            // if (index == -1)
            //     throw new Exception("Object not on this diagram.");

            // ParentObjs[index] = Obj;
        }
    }
}