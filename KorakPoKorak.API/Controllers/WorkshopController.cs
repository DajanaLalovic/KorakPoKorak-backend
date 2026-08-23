using System.Security.Claims;
using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/workshop")]
    [Authorize]
    public class WorkshopController : ControllerBase
    {
        private readonly IWorkshopService _service;

        public WorkshopController(IWorkshopService service)
        {
            _service = service;
        }

        /// <summary>Returns paginated workshops with optional filters. All authenticated users.</summary>
        [HttpGet]
        public IActionResult GetAll([FromQuery] WorkshopQueryParams q)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (q.OnlyMine)
                q.CreatedById = userId;
            if (q.ContributorId.HasValue && q.ContributorId.Value == -1)
                q.ContributorId = userId;
            return Ok(_service.GetFiltered(q));
        }

        /// <summary>Returns workshops created by the current user.</summary>
        [HttpGet("my")]
        public IActionResult GetMy()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetMy(userId));
        }

        /// <summary>Unique students enrolled in the current mentor's workshops (active + completed).</summary>
        [HttpGet("my/student-count")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult GetMyStudentCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_service.GetMyStudentCount(userId));
        }

        /// <summary>Returns total, published, and draft workshop counts in one call.</summary>
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            return Ok(_service.GetSummary());
        }

        /// <summary>Returns the N most recently created workshops.</summary>
        [HttpGet("recent")]
        public IActionResult GetRecent([FromQuery] int count = 5)
        {
            return Ok(_service.GetRecent(count));
        }

        /// <summary>Returns a workshop by ID. All authenticated users.</summary>
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var workshop = _service.GetById(id);
            return workshop == null ? NotFound() : Ok(workshop);
        }

        /// <summary>Returns all lessons that belong to a workshop.</summary>
        [HttpGet("{id:int}/lessons")]
        public IActionResult GetLessons(int id)
        {
            return Ok(_service.GetLessons(id));
        }

        /// <summary>Returns all exercises that belong to a workshop.</summary>
        [HttpGet("{id:int}/exercises")]
        public IActionResult GetExercises(int id)
        {
            return Ok(_service.GetExercises(id));
        }

        /// <summary>Returns students enrolled in this workshop (active + completed).</summary>
        [HttpGet("{id:int}/students")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult GetStudents(int id)
        {
            try
            {
                return Ok(_service.GetStudents(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Returns lesson/exercise counts and enrollment stats for a workshop.</summary>
        [HttpGet("{id:int}/stats")]
        public IActionResult GetStats(int id)
        {
            try
            {
                return Ok(_service.GetStats(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Creates a new workshop. Administrator and Mentor only.</summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Create([FromBody] CreateWorkshopDto dto)
        {
            var createdById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _service.Create(dto, createdById);
            return Ok();
        }

        /// <summary>Updates a workshop. Administrator and Mentor only.</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Update(int id, [FromBody] UpdateWorkshopDto dto)
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

        /// <summary>Changes only the status of a workshop (Draft/Published/Archived). Administrator and Mentor only.</summary>
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult PatchStatus(int id, [FromBody] PatchWorkshopStatusDto dto)
        {
            try
            {
                _service.PatchStatus(id, dto.Status);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Attaches an existing lesson to a workshop. Administrator and Mentor only.</summary>
        [HttpPost("{id:int}/lessons")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult AttachLesson(int id, [FromBody] AttachLessonDto dto)
        {
            try
            {
                _service.AttachLesson(id, dto.LessonId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>Detaches a lesson from a workshop. Administrator and Mentor only.</summary>
        [HttpDelete("{id:int}/lessons/{lessonId:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult DetachLesson(int id, int lessonId)
        {
            _service.DetachLesson(id, lessonId);
            return Ok();
        }

        /// <summary>Attaches an existing exercise to a workshop. Administrator and Mentor only.</summary>
        [HttpPost("{id:int}/exercises")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult AttachExercise(int id, [FromBody] AttachExerciseDto dto)
        {
            try
            {
                _service.AttachExercise(id, dto.ExerciseId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>Detaches an exercise from a workshop. Administrator and Mentor only.</summary>
        [HttpDelete("{id:int}/exercises/{exerciseId:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult DetachExercise(int id, int exerciseId)
        {
            _service.DetachExercise(id, exerciseId);
            return Ok();
        }

        /// <summary>Deletes a workshop. Administrator and Mentor only.</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Mentor")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
