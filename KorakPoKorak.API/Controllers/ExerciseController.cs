using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/exercise")]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _service;

        public ExerciseController(IExerciseService service)
        {
            _service = service;
        }

        /// <summary>Returns paginated exercises with optional search and printable filter. All authenticated users.</summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] ExerciseQueryParams q)
        {
            return Ok(_service.GetFiltered(q));
        }

        /// <summary>Returns exercises created by the current user.</summary>
        [HttpGet("my")]
        public IActionResult GetMy()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetMy(userId));
        }

        /// <summary>Returns the N most recently created exercises.</summary>
        [HttpGet("recent")]
        public IActionResult GetRecent([FromQuery] int count = 5)
        {
            return Ok(_service.GetRecent(count));
        }

        /// <summary>Returns an exercise by ID including content blocks. All authenticated users.</summary>
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var exercise = _service.GetById(id);
            return exercise == null ? NotFound() : Ok(exercise);
        }

        /// <summary>Creates a new exercise (optional contentBlocks). Administrator and Mentor only.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Create([FromBody] CreateExerciseDto dto)
        {
            var createdById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                return Ok(_service.Create(dto, createdById));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Updates an exercise. Administrator and Mentor only.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Update(int id, [FromBody] UpdateExerciseDto dto)
        {
            try
            {
                return Ok(_service.Update(id, dto));
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

        /// <summary>Deletes an exercise. Administrator and Mentor only.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
