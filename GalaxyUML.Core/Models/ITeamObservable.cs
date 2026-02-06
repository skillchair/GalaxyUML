using GalaxyUML.Core.Models.Commands.TeamCommands;

namespace GalaxyUML.Core.Models
{
    public interface ITeamObservable
    {
        void Attach(ITeamObserver observer);
        void Detach(ITeamObserver observer);
        void Notify(TeamEventType eventType, ITeamCommand command);
    }
}