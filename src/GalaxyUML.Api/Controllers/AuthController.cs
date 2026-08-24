// exposes anonymous registration and login endpoints.

using System.ComponentModel.DataAnnotations;
using GalaxyUML.Api.Services;
using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _users;
    private readonly TokenService _tokens;

    public AuthController(UserService users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var id = await _users.RegisterAsync(dto.FirstName, dto.LastName, dto.Username, dto.Email, dto.Password);
            return Ok(id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _users.ValidateAsync(dto.Username, dto.Password);
        if (user is null)
        {
            return Unauthorized();
        }

        var token = _tokens.Create(user);
        return Ok(new { token, user = new { user.IdUser, user.Username, user.Email } });
    }
}

public record RegisterDto(
    [Required, MinLength(1), MaxLength(80)] string FirstName,
    [Required, MinLength(1), MaxLength(80)] string LastName,
    [Required, MinLength(3), MaxLength(80)] string Username,
    [Required, EmailAddress, MaxLength(200)] string Email,
    [Required, MinLength(8), MaxLength(200)] string Password);

public record LoginDto(
    [Required] string Username,
    [Required] string Password);
