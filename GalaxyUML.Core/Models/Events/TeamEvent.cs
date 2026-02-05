namespace GalaxyUML.Core.Models.Events
{
    public class TeamEvent<T>
    {
        // T je npr User koji se upravo prikljucio; prosledjujemo promenu tako sto on zadrzava svoj primarni kljuc
        // i nosi promenu i uspesno ce se azurirati;
        public TeamEventType Type { get; private set; }
        public T Obj { get; set; }

        public TeamEvent(TeamEventType type, T obj)
        {
            Type = type;
            Obj = obj;
        }
    }
}