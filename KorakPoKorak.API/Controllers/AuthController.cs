using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Logs in with email and password. Returns a JWT token.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = _authService.Login(dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Registers a new user. The default assigned role is 'child'.
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            try
            {
                _authService.Register(dto);
                return Ok(new { message = "Registration successful." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
