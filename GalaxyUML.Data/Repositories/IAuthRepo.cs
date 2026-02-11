using GalaxyUML.Core.Models;
using GalaxyUML.Core.Security;

namespace GalaxyUML.Data.Repositories
{
    public interface IAuthRepo
    {
        Task<User?> LoginAsync(string username, string password);
    }
}