using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/child")]
    [Authorize(Roles = "Parent")]
    public class ChildController : ControllerBase
    {
        private readonly IChildService _service;

        public ChildController(IChildService service)
        {
            _service = service;
        }

        /// <summary>Returns all children belonging to the currently logged-in Parent.</summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetMyChildren(parentId));
        }

        /// <summary>Returns a single child, only if it belongs to the logged-in Parent.</summary>
        [HttpGet("{childId:int}")]
        public IActionResult Get(int childId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var child = _service.GetById(childId, parentId);
            return child == null ? NotFound() : Ok(child);
        }

        /// <summary>Creates a new child profile for the logged-in Parent.</summary>
        [HttpPost]
        public IActionResult Create([FromBody] CreateChildDto dto)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var created = _service.Create(dto, parentId);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Updates a child, only if it belongs to the logged-in Parent.</summary>
        [HttpPut("{childId:int}")]
        public IActionResult Update(int childId, [FromBody] UpdateChildDto dto)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var updated = _service.Update(childId, dto, parentId);
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

        /// <summary>Uploads or replaces a child's avatar image. Multipart form field name: file.</summary>
        [HttpPost("{childId:int}/avatar")]
        [RequestSizeLimit(3 * 1024 * 1024)]
        public IActionResult UploadAvatar(int childId, [FromForm] IFormFile file)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Avatar file is required." });

            try
            {
                using var stream = file.OpenReadStream();
                var updated = _service.UploadAvatar(childId, parentId, stream, file.ContentType, file.Length);
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

        /// <summary>Deletes a child, only if it belongs to the logged-in Parent.</summary>
        [HttpDelete("{childId:int}")]
        public IActionResult Delete(int childId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                _service.Delete(childId, parentId);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
