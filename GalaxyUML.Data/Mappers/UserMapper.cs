using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class UserMapper
{
    public static User ToDomain(UserEntity e) =>
        new User(e.Id, e.FirstName, e.LastName, e.Username, e.Email, e.Password);

    public static UserEntity ToEntity(User d) => new()
    {
        Id = d.IdUser,
        FirstName = d.FirstName,
        LastName = d.LastName,
        Username = d.Username,
        Email = d.Email,
        Password = d.Password
    };
}
