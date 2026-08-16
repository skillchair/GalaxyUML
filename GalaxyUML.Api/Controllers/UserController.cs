// exposes the authenticated user's public profile.

using GalaxyUML.Api.Security;
using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(UserService users, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrent()
    {
        var user = await users.GetAsync(currentUser.Id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(new UserResponse(user.IdUser, user.FirstName, user.LastName, user.Username, user.Email));
    }
}

public record UserResponse(Guid Id, string FirstName, string LastName, string Username, string Email);
