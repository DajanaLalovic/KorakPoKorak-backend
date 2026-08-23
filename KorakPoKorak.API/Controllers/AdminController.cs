using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWorkshopService _workshopService;
        private readonly ILessonService _lessonService;
        private readonly IExerciseService _exerciseService;

        public AdminController(
            IUserService userService,
            IWorkshopService workshopService,
            ILessonService lessonService,
            IExerciseService exerciseService)
        {
            _userService = userService;
            _workshopService = workshopService;
            _lessonService = lessonService;
            _exerciseService = exerciseService;
        }

        // ─── STATS ────────────────────────────────────────────────────────────

        /// <summary>Returns platform-wide stats: user counts, workshop counts, lesson/exercise totals.</summary>
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var users = _userService.GetAll();
            var workshopSummary = _workshopService.GetSummary();
            var lessonTotal = _lessonService.GetFiltered(new LessonQueryParams { Page = 0, Size = 1 }).TotalElements;
            var exerciseTotal = _exerciseService.GetFiltered(new ExerciseQueryParams { Page = 0, Size = 1 }).TotalElements;

            var stats = new AdminStatsDto
            {
                TotalUsers = users.Count,
                TotalAdministrators = users.Count(u => u.Role.RoleName == "Administrator"),
                TotalMentors = users.Count(u => u.Role.RoleName == "Mentor"),
                TotalChildren = users.Count(u => u.Role.RoleName == "Child"),
                TotalWorkshops = workshopSummary.Total,
                PublishedWorkshops = workshopSummary.Published,
                DraftWorkshops = workshopSummary.Draft,
                ArchivedWorkshops = workshopSummary.Total - workshopSummary.Published - workshopSummary.Draft,
                TotalLessons = lessonTotal,
                TotalExercises = exerciseTotal
            };

            return Ok(stats);
        }

        // ─── USERS ────────────────────────────────────────────────────────────

        /// <summary>Returns all users.</summary>
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAll());
        }

        /// <summary>Returns a single user by ID.</summary>
        [HttpGet("users/{id:int}")]
        public IActionResult GetUser(int id)
        {
            var user = _userService.GetById(id);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>Creates a new user with any role.</summary>
        [HttpPost("users")]
        public IActionResult CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                _userService.Create(dto);
                return Ok(new { message = "User created." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Updates a user's basic info (name, email, phone).</summary>
        [HttpPut("users/{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                _userService.Update(id, dto);
                return Ok(new { message = "User updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Changes a user's role (Administrator, Mentor, Child).</summary>
        [HttpPatch("users/{id:int}/role")]
        public IActionResult ChangeUserRole(int id, [FromBody] ChangeUserRoleDto dto)
        {
            try
            {
                _userService.ChangeRole(id, dto.Role);
                return Ok(new { message = "Role updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Activates or deactivates a user account.</summary>
        [HttpPatch("users/{id:int}/active")]
        public IActionResult SetUserActive(int id, [FromBody] SetActiveDto dto)
        {
            try
            {
                _userService.SetActive(id, dto.IsActive);
                return Ok(new { message = dto.IsActive ? "User activated." : "User deactivated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Permanently deletes a user.</summary>
        [HttpDelete("users/{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            _userService.Delete(id);
            return Ok(new { message = "User deleted." });
        }

        // ─── WORKSHOPS ────────────────────────────────────────────────────────

        /// <summary>Returns all workshops with optional filters.</summary>
        [HttpGet("workshops")]
        public IActionResult GetAllWorkshops([FromQuery] WorkshopQueryParams q)
        {
            return Ok(_workshopService.GetFiltered(q));
        }

        /// <summary>Returns a single workshop by ID.</summary>
        [HttpGet("workshops/{id:int}")]
        public IActionResult GetWorkshop(int id)
        {
            var workshop = _workshopService.GetById(id);
            return workshop == null ? NotFound() : Ok(workshop);
        }

        /// <summary>Updates a workshop.</summary>
        [HttpPut("workshops/{id:int}")]
        public IActionResult UpdateWorkshop(int id, [FromBody] UpdateWorkshopDto dto)
        {
            try
            {
                _workshopService.Update(id, dto);
                return Ok(new { message = "Workshop updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Changes the status of a workshop (Draft, Published, Archived).</summary>
        [HttpPatch("workshops/{id:int}/status")]
        public IActionResult PatchWorkshopStatus(int id, [FromBody] PatchWorkshopStatusDto dto)
        {
            try
            {
                _workshopService.PatchStatus(id, dto.Status);
                return Ok(new { message = "Workshop status updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Permanently deletes a workshop.</summary>
        [HttpDelete("workshops/{id:int}")]
        public IActionResult DeleteWorkshop(int id)
        {
            _workshopService.Delete(id);
            return Ok(new { message = "Workshop deleted." });
        }

        // ─── LESSONS ──────────────────────────────────────────────────────────

        /// <summary>Returns all lessons with optional filters.</summary>
        [HttpGet("lessons")]
        public IActionResult GetAllLessons([FromQuery] LessonQueryParams q)
        {
            return Ok(_lessonService.GetFiltered(q));
        }

        /// <summary>Returns a single lesson by ID.</summary>
        [HttpGet("lessons/{id:int}")]
        public IActionResult GetLesson(int id)
        {
            var lesson = _lessonService.GetById(id);
            return lesson == null ? NotFound() : Ok(lesson);
        }

        /// <summary>Updates a lesson.</summary>
        [HttpPut("lessons/{id:int}")]
        public IActionResult UpdateLesson(int id, [FromBody] UpdateLessonDto dto)
        {
            try
            {
                _lessonService.Update(id, dto);
                return Ok(new { message = "Lesson updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Permanently deletes a lesson.</summary>
        [HttpDelete("lessons/{id:int}")]
        public IActionResult DeleteLesson(int id)
        {
            _lessonService.Delete(id);
            return Ok(new { message = "Lesson deleted." });
        }

        // ─── EXERCISES ────────────────────────────────────────────────────────

        /// <summary>Returns all exercises with optional filters.</summary>
        [HttpGet("exercises")]
        public IActionResult GetAllExercises([FromQuery] ExerciseQueryParams q)
        {
            return Ok(_exerciseService.GetFiltered(q));
        }

        /// <summary>Returns a single exercise by ID.</summary>
        [HttpGet("exercises/{id:int}")]
        public IActionResult GetExercise(int id)
        {
            var exercise = _exerciseService.GetById(id);
            return exercise == null ? NotFound() : Ok(exercise);
        }

        /// <summary>Updates an exercise.</summary>
        [HttpPut("exercises/{id:int}")]
        public IActionResult UpdateExercise(int id, [FromBody] UpdateExerciseDto dto)
        {
            try
            {
                _exerciseService.Update(id, dto);
                return Ok(new { message = "Exercise updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Permanently deletes an exercise.</summary>
        [HttpDelete("exercises/{id:int}")]
        public IActionResult DeleteExercise(int id)
        {
            _exerciseService.Delete(id);
            return Ok(new { message = "Exercise deleted." });
        }
    }
}
