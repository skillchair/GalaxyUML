namespace GalaxyUML.Core
{
    public interface ITeamObserver<T>
    {
        public abstract void Update(TeamEvent<T> teamEvent);
    }
}