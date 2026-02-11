using Microsoft.AspNetCore.Mvc;
using IAuthRepo = GalaxyUML.Data.Repositories.IAuthRepo;
using LoginDto = GalaxyUML.Core.Models.LoginDto;

namespace GalaxyUML.Api
{
    [ApiController]
    [Route("api/auths")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;

        public AuthController(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto login)
        {
            var user = await _authRepo.LoginAsync(login.Username, login.Password);

            if (user == null)
            {
                // Vraćamo 401 Unauthorized ako korisnik ne postoji ili je šifra loša
                return Unauthorized("Bad login creditentials.");
            }

            // Ako je sve OK, vrati korisnika (ili kasnije JWT token)
            return Ok(user);
        }
    }
}