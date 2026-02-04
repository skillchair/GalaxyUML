namespace GalaxyUML.Core.Models
{
    public interface ITeamObserver<T>
    {
        public abstract void Update(Events.TeamEvent<T> teamEvent);
    }
}