namespace GalaxyUML.Core
{
    interface ITeamObservable
    {
        void Attach(ITeamObserver observer);
        void Detach(ITeamObserver observer);
        void Notify();
    }
}