namespace GalaxyUML.Core
{
    public class TeamEvent<T>
    {
        public TeamEventType TeamEventType { get; private set; }
        public T Obj { get; set; }

        public TeamEvent(TeamEventType type, T obj)
        {
            TeamEventType = type;
            Obj = obj;
        }
    }
}