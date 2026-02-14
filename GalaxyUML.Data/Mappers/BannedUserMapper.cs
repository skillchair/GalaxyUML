using GalaxyUML.Core.Models;
using GalaxyUML.Data.Entities;

namespace GalaxyUML.Data.Mappers;

public static class BannedUserMapper
{
    public static BannedUser ToDomain(BannedUserEntity e) =>
        new BannedUser(e.UserId, e.Reason);

    public static BannedUserEntity ToEntity(Guid teamId, BannedUser d) => new()
    {
        Id = Guid.NewGuid(),
        TeamId = teamId,
        UserId = d.UserId,
        BannedAt = d.BannedAt,
        Reason = d.Reason
    };
}
