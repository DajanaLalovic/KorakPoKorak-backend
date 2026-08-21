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
        private readonly IChildService _childService;

        public MentorController(IUserService userService, IChildService childService)
        {
            _userService = userService;
            _childService = childService;
        }

        /// <summary>Returns all users with the Mentor role. Used for contributor pickers.</summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetMentors());
        }

        /// <summary>Returns all active child profiles so mentors can enroll them in workshops.</summary>
        [HttpGet("students")]
        public IActionResult GetStudents()
        {
            return Ok(_childService.GetAllForMentor());
        }
    }
}
