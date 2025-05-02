using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TheatreManagementSystem.DTOs;
using TheatreManagementSystem.Services;

namespace TheatreManagementSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var (success, message) = await _authService.RegisterUserAsync(userDTO);
                if (success)
                    return Ok(new { message });
                else
                    return BadRequest(message);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var (success, token, message) = await _authService.LoginAsync(loginRequest.Username, loginRequest.Password);
                if (success)
                    return Ok(new { token, message });
                else
                    return Unauthorized(new { message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }

    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}