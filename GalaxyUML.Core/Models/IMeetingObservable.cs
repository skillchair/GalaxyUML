namespace GalaxyUML.Core.Models
{
    public interface IMeetingObservable<T>
    {
        void Attach(IMeetingObserver meetingObserver);
        void Detach(IMeetingObserver meetingObserver);
        void Notify(Events.MeetingEvent<T> meetingEvent);
    }
}