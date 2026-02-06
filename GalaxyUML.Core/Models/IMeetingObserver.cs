using GalaxyUML.Core.Models.Commands.MeetingCommands;

namespace GalaxyUML.Core.Models
{
    public interface IMeetingObserver
    {
        void Update(MeetingEventType eventType, IMeetingCommand command);
    }
}