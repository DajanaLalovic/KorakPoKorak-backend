using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/mentor")]
    [Authorize(Roles = "Administrator,Mentor")]
    public class MentorController : ControllerBase
    {
        private readonly IUserService _userService;

        public MentorController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Returns all users with the Mentor role. Used for contributor pickers.</summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetMentors());
        }
    }
}
