namespace GalaxyUML.Core
{
    interface IMeetingObservable<T>
    {
        void Attach(IMeetingObserver meetingObserver);
        void Detach(IMeetingObserver meetingObserver);
        void Notify(MeetingEvent<T> meetingEvent);
    }
}