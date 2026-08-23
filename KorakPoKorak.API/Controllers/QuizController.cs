using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/quiz")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _service;

        public QuizController(IQuizService service)
        {
            _service = service;
        }

        /// <summary>Returns paginated quizzes with optional title search.</summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] QuizQueryParams q)
        {
            if (q.OnlyMine)
                q.CreatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetFiltered(q));
        }

        /// <summary>Returns quizzes created by the current user.</summary>
        [HttpGet("my")]
        public IActionResult GetMy()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetMy(userId));
        }

        /// <summary>Returns a single quiz with all questions and answers.</summary>
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var quiz = _service.GetById(id);
            return quiz == null ? NotFound() : Ok(quiz);
        }

        /// <summary>Creates a new quiz. Mentor and Administrator only.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Create([FromBody] CreateQuizDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = _service.Create(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        /// <summary>Replaces a quiz's title and all its questions/answers.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Update(int id, [FromBody] UpdateQuizDto dto)
        {
            try
            {
                var result = _service.Update(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Deletes a quiz and all its questions/answers.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
