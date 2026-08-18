using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/lesson")]
    [Authorize]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _service;

        public LessonController(ILessonService service)
        {
            _service = service;
        }

        /// <summary>Returns paginated lessons with optional search. All authenticated users.</summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] LessonQueryParams q)
        {
            if (q.OnlyMine)
                q.CreatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetFiltered(q));
        }

        /// <summary>Returns lessons created by the current user.</summary>
        [HttpGet("my")]
        public IActionResult GetMy()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetMy(userId));
        }

        /// <summary>Returns the N most recently created lessons.</summary>
        [HttpGet("recent")]
        public IActionResult GetRecent([FromQuery] int count = 5)
        {
            return Ok(_service.GetRecent(count));
        }

        /// <summary>Returns a lesson by ID. All authenticated users.</summary>
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var lesson = _service.GetById(id);
            return lesson == null ? NotFound() : Ok(lesson);
        }

        /// <summary>Creates a new lesson. Administrator and Mentor only.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Create([FromBody] CreateLessonDto dto)
        {
            var createdById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _service.Create(dto, createdById);
            return Ok();
        }

        /// <summary>Updates a lesson. Administrator and Mentor only.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Update(int id, [FromBody] UpdateLessonDto dto)
        {
            try
            {
                _service.Update(id, dto);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Deletes a lesson. Administrator and Mentor only.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
