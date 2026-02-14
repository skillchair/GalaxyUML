using GalaxyUML.Api.Services;
using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

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
        var id = await _users.RegisterAsync(dto.FirstName, dto.LastName, dto.Username, dto.Email, dto.Password);
        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _users.ValidateAsync(dto.Username, dto.Password);
        if (user is null) return Unauthorized();
        var token = _tokens.Create(user);
        return Ok(new { token, user = new { user.IdUser, user.Username, user.Email } });
    }
}

public record RegisterDto(string FirstName, string LastName, string Username, string Email, string Password);
public record LoginDto(string Username, string Password);
