using GalaxyUML.Core.Models.Commands.TeamCommands;

namespace GalaxyUML.Core.Models
{
    public interface ITeamObserver
    {
        public void Update(TeamEventType eventType, ITeamCommand command);
    }
}