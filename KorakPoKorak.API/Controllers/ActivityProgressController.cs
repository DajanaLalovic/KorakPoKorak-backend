using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/child/{childId:int}")]
    [Authorize(Roles = "Parent")]
    public class ActivityProgressController : ControllerBase
    {
        private readonly IActivityProgressService _service;

        public ActivityProgressController(IActivityProgressService service)
        {
            _service = service;
        }

        /// <summary>Returns activity progress for a child's enrollment.</summary>
        [HttpGet("enrollment/{enrollmentId:int}/progress")]
        public IActionResult GetByEnrollment(int childId, int enrollmentId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                return Ok(_service.GetByEnrollment(enrollmentId, childId, parentId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Returns activity progress for a child in a workshop (requires enrollment).</summary>
        [HttpGet("workshop/{workshopId:int}/progress")]
        public IActionResult GetByWorkshop(int childId, int workshopId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                return Ok(_service.GetByChildAndWorkshop(childId, workshopId, parentId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Creates or updates Lesson/Exercise progress for an enrolled child/workshop.</summary>
        [HttpPut("workshop/{workshopId:int}/progress")]
        public IActionResult Upsert(int childId, int workshopId, [FromBody] UpdateActivityProgressDto dto)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var updated = _service.Upsert(childId, workshopId, parentId, dto);
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
