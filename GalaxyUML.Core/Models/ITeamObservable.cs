namespace GalaxyUML.Core.Models
{
    public interface ITeamObservable<T>
    {
        void Attach(ITeamObserver<T> observer);
        void Detach(ITeamObserver<T> observer);
        void Notify(TeamEventType teamEvent);
    }
}