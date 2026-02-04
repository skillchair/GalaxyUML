namespace GalaxyUML.Core.Models
{
    public interface IMeetingObserver<T>
    {
        void Update(MeetingEventType meetingEvent);
    }
}