using GalaxyUML.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyUML.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _svc;
    public UsersController(UserService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var id = await _svc.RegisterAsync(dto.FirstName, dto.LastName, dto.Username, dto.Email, dto.Password);
        return Ok(id);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _svc.GetAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
}

