using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/child/{childId:int}/enrollment")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        /// <summary>Enrolls a child into a workshop. Parents can enroll their own children; mentors can enroll any child.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Mentor,Parent")]
        public IActionResult Enroll(int childId, [FromBody] CreateEnrollmentDto dto)
        {
            try
            {
                EnrollmentDto created;
                if (User.IsInRole("Mentor") || User.IsInRole("Administrator"))
                    created = _service.EnrollAsMentor(childId, dto);
                else
                {
                    var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    created = _service.Enroll(childId, parentId, dto);
                }
                return Ok(created);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Lists enrollments/workshops for the Parent's child.</summary>
        [HttpGet]
        [Authorize(Roles = "Parent")]
        public IActionResult GetAll(int childId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                return Ok(_service.GetByChild(childId, parentId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Returns one enrollment for the Parent's child.</summary>
        [HttpGet("{enrollmentId:int}")]
        [Authorize(Roles = "Parent")]
        public IActionResult Get(int childId, int enrollmentId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var enrollment = _service.GetById(enrollmentId, childId, parentId);
                return enrollment == null ? NotFound() : Ok(enrollment);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Updates enrollment status (Active / Completed / Withdrawn).</summary>
        [HttpPatch("{enrollmentId:int}/status")]
        [Authorize(Roles = "Parent")]
        public IActionResult UpdateStatus(int childId, int enrollmentId, [FromBody] UpdateEnrollmentStatusDto dto)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var updated = _service.UpdateStatus(enrollmentId, childId, parentId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
