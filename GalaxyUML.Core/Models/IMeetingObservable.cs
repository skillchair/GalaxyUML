using GalaxyUML.Core.Models.Commands.MeetingCommands;

namespace GalaxyUML.Core.Models
{
    public interface IMeetingObservable
    {
        void Attach(IMeetingObserver observer);
        void Detach(IMeetingObserver oserver);
        void Notify(MeetingEventType eventType, IMeetingCommand command);
    }
}