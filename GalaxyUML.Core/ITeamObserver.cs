namespace GalaxyUML.Core
{
    interface ITeamObserver<T>
    {
        public abstract void Update(TeamEvent<T> teamEvent);
    }
}