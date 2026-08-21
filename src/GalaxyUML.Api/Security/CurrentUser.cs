// provides one validated source for the authenticated api user.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GalaxyUML.Api.Security;

public interface ICurrentUser
{
    Guid Id { get; }
    Guid GetId(ClaimsPrincipal? principal);
}

public sealed class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid Id => GetId(httpContextAccessor.HttpContext?.User);

    public Guid GetId(ClaimsPrincipal? principal)
    {
        var subject = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (principal?.Identity?.IsAuthenticated != true || !Guid.TryParse(subject, out var userId))
        {
            throw new InvalidOperationException("The authenticated user identifier is missing or invalid.");
        }

        return userId;
    }
}
