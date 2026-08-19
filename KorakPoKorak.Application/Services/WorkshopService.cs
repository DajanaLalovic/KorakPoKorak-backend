using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IWorkshopRepository _repo;

        public WorkshopService(IWorkshopRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<WorkshopDto> GetFiltered(WorkshopQueryParams q)
        {
            var (items, total) = _repo.GetFiltered(q);
            return new PagedResult<WorkshopDto>
            {
                Content = items.Select(MapToDto).ToList(),
                TotalElements = total,
                Page = q.Page,
                Size = q.Size
            };
        }

        public WorkshopSummaryDto GetSummary()
        {
            var (total, published, draft) = _repo.GetCounts();
            return new WorkshopSummaryDto { Total = total, Published = published, Draft = draft };
        }

        public WorkshopDto? GetById(int id)
        {
            var workshop = _repo.GetById(id);
            return workshop == null ? null : MapToDto(workshop);
        }

        public List<WorkshopDto> GetMy(int userId)
        {
            return _repo.GetMy(userId).Select(MapToDto).ToList();
        }

        public List<WorkshopDto> GetRecent(int count)
        {
            return _repo.GetRecent(count).Select(MapToDto).ToList();
        }

        public List<LessonDto> GetLessons(int workshopId)
        {
            return _repo.GetWorkshopLessons(workshopId).Select(MapLessonToDto).ToList();
        }

        public List<ExerciseDto> GetExercises(int workshopId)
        {
            return _repo.GetWorkshopExercises(workshopId).Select(MapExerciseToDto).ToList();
        }

        public WorkshopStatsDto GetStats(int id)
        {
            var workshop = _repo.GetById(id)
                ?? throw new KeyNotFoundException($"Workshop with id {id} not found.");

            return new WorkshopStatsDto
            {
                WorkshopId = id,
                LessonCount = workshop.Lessons.Count,
                ExerciseCount = workshop.Exercises.Count,
                EnrollmentCount = 0,
                CompletionRate = 0
            };
        }

        public void Create(CreateWorkshopDto dto, int createdById)
        {
            var workshop = new Workshop
            {
                Title = dto.Title,
                Description = dto.Description,
                AgeMin = dto.AgeMin,
                AgeMax = dto.AgeMax,
                ComplexityLevel = dto.ComplexityLevel,
                ActivityTypes = dto.ActivityTypes,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };

            _repo.Add(workshop, dto.LessonIds, dto.ExerciseIds);
        }

        public void Update(int id, UpdateWorkshopDto dto)
        {
            var workshop = _repo.GetById(id)
                ?? throw new KeyNotFoundException($"Workshop with id {id} not found.");

            workshop.Title = dto.Title;
            workshop.Description = dto.Description;
            workshop.AgeMin = dto.AgeMin;
            workshop.AgeMax = dto.AgeMax;
            workshop.ComplexityLevel = dto.ComplexityLevel;
            workshop.ActivityTypes = dto.ActivityTypes;
            workshop.Status = dto.Status;

            _repo.Update(workshop, dto.LessonIds, dto.ExerciseIds);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public void PatchStatus(int id, WorkshopStatus status)
        {
            _repo.PatchStatus(id, status);
        }

        public void AttachLesson(int workshopId, int lessonId)
        {
            _repo.AttachLesson(workshopId, lessonId);
        }

        public void DetachLesson(int workshopId, int lessonId)
        {
            _repo.DetachLesson(workshopId, lessonId);
        }

        public void AttachExercise(int workshopId, int exerciseId)
        {
            _repo.AttachExercise(workshopId, exerciseId);
        }

        public void DetachExercise(int workshopId, int exerciseId)
        {
            _repo.DetachExercise(workshopId, exerciseId);
        }

        private static WorkshopDto MapToDto(Workshop w) => new()
        {
            Id = w.Id,
            Title = w.Title,
            Description = w.Description,
            AgeMin = w.AgeMin,
            AgeMax = w.AgeMax,
            ComplexityLevel = w.ComplexityLevel,
            ActivityTypes = w.ActivityTypes,
            Status = w.Status,
            CreatedAt = w.CreatedAt,
            CreatedById = w.CreatedById,
            CreatedByName = $"{w.CreatedBy.FirstName} {w.CreatedBy.LastName}",
            LessonIds = w.Lessons.Select(l => l.Id).ToList(),
            ExerciseIds = w.Exercises.Select(e => e.Id).ToList()
        };

        private static LessonDto MapLessonToDto(Lesson l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            EstimatedTime = l.EstimatedTime,
            CreatedAt = l.CreatedAt,
            CreatedById = l.CreatedById,
            CreatedByName = $"{l.CreatedBy.FirstName} {l.CreatedBy.LastName}",
            ContentBlocks = ContentBlockMapper.ToDtoList(l.ContentBlocks)
        };

        private static ExerciseDto MapExerciseToDto(Exercise e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            EstimatedTime = e.EstimatedTime,
            IsPrintable = e.IsPrintable,
            CreatedAt = e.CreatedAt,
            CreatedById = e.CreatedById,
            CreatedByName = $"{e.CreatedBy.FirstName} {e.CreatedBy.LastName}",
            ContentBlocks = ContentBlockMapper.ToDtoList(e.ContentBlocks)
        };
    }
}
