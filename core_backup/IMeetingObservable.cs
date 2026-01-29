namespace GalaxyUML.Core
{
    interface IMeetingObservable
    {
        void Attach(IMeetingObserver meetingObserver);
        void Detach(IMeetingObserver meetingObserver);
        void Notify();
    }
}