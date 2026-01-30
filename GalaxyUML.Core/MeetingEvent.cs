namespace GalaxyUML.Core
{
    class MeetingEvent<T>
    {
        public MeetingEventType MeetingEventType { get; private set; }
        public T Obj { get; private set; }

        public MeetingEvent(MeetingEventType type, T obj)
        {
            MeetingEventType = type;
            Obj = obj;
        }
    }
}