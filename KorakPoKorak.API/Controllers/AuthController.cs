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
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Registers a new inactive user and sends an activation email.
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            try
            {
                var response = _authService.Register(dto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activates a user account using the token from the confirmation email.
        /// </summary>
        [HttpPost("activate")]
        public IActionResult Activate([FromBody] ActivateAccountDto dto)
        {
            try
            {
                _authService.ActivateAccount(dto.Token);
                return Ok(new { message = "Account activated. You can now log in." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Stub: sends a password-reset email. Returns success regardless (email service not yet implemented).
        /// </summary>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            // Email delivery is not implemented yet. Return 200 to avoid leaking
            // whether a given address exists in the system.
            return Ok(new { success = true });
        }
    }
}
