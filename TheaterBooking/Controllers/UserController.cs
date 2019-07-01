using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheaterBooking.Models.Users;
using TheaterBooking.Services;

namespace TheaterBooking.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _service;

        public UsersController(UsersService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _service.RegisterAsync(user);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _service.LoginAsync(login);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUsers([FromQuery] string userName, [FromQuery] string role)
        {
            return Ok(await _service.GetAllUsersAsync(userName, role));
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            await _service.ConfirmEmail(userId, token);

            return Redirect("/");
        }

    }
}
