using BannedUser = GalaxyUML.Core.Models.BannedUser;

namespace GalaxyUML.Data.Repositories
{
    interface IBannedUserRepo
    {
        Task<BannedUser?> GetByIdAsync(Guid id);
        Task<IEnumerable<BannedUser>> GetByUserAsync(Guid idUser);
        Task<IEnumerable<BannedUser>> GetByTeamAsync(Guid idTeam);
        Task<IEnumerable<BannedUser>> GetAllAsync();

        Task CreateAsync(BannedUser bannedUser);
        Task UpdateAsync(BannedUser bannedUser);
        Task DeleteAsync(Guid id);
    }
}