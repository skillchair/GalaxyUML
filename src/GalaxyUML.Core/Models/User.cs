namespace GalaxyUML.Core.Models
{
    public class User
    {
        public Guid IdUser { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        public string Email { get; }
        public string Password { get; }

        public User(Guid idUser, string firstName, string lastName, string username, string email, string password)
        {
            IdUser = idUser;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
            Password = password;
        }
    }
}
