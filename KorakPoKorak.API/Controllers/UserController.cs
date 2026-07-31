using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        /// <summary>Returns all users. Administrator only.</summary>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        /// <summary>
        /// Returns a user by ID.
        /// Administrator sees anyone; Mentor sees children + themselves; Child sees only themselves.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var requesterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var requesterRole = User.FindFirstValue(ClaimTypes.Role)!;

            if (requesterRole == nameof(UserRole.Administrator))
            {
                var user = _service.GetById(id);
                return user == null ? NotFound() : Ok(user);
            }

            if (requesterId == id)
            {
                var user = _service.GetById(id);
                return user == null ? NotFound() : Ok(user);
            }

            if (requesterRole == nameof(UserRole.Mentor))
            {
                var targetUser = _service.GetById(id);
                if (targetUser != null && targetUser.Role.RoleName == nameof(UserRole.Child))
                    return Ok(targetUser);
            }

            return Forbid();
        }

        /// <summary>Creates a new user with a specified role. Administrator only.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([FromBody] CreateUserDto dto)
        {
            _service.Create(dto);
            return Ok();
        }

        /// <summary>Deletes a user by ID. Administrator only.</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
