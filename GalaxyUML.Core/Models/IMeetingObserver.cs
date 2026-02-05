namespace GalaxyUML.Core.Models
{
    public interface IMeetingObserver
    {
        void Update(MeetingEventType meetingEvent);
    }
}