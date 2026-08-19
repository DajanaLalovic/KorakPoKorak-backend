using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _repo;

        public ExerciseService(IExerciseRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<ExerciseDto> GetFiltered(ExerciseQueryParams q)
        {
            var (items, total) = _repo.GetFiltered(q);
            return new PagedResult<ExerciseDto>
            {
                Content = items.Select(MapToDto).ToList(),
                TotalElements = total,
                Page = q.Page,
                Size = q.Size
            };
        }

        public ExerciseDto? GetById(int id)
        {
            var exercise = _repo.GetById(id);
            return exercise == null ? null : MapToDto(exercise);
        }

        public List<ExerciseDto> GetMy(int userId)
        {
            return _repo.GetMy(userId).Select(MapToDto).ToList();
        }

        public List<ExerciseDto> GetRecent(int count)
        {
            return _repo.GetRecent(count).Select(MapToDto).ToList();
        }

        public void Create(CreateExerciseDto dto, int createdById)
        {
            var exercise = new Exercise
            {
                Title = dto.Title,
                EstimatedTime = dto.EstimatedTime,
                IsPrintable = dto.IsPrintable,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };
            _repo.Add(exercise);
        }

        public void Update(int id, UpdateExerciseDto dto)
        {
            var exercise = _repo.GetById(id)
                ?? throw new KeyNotFoundException($"Exercise with id {id} not found.");
            exercise.Title = dto.Title;
            exercise.EstimatedTime = dto.EstimatedTime;
            exercise.IsPrintable = dto.IsPrintable;
            exercise.Status = dto.Status;
            _repo.Update(exercise);
        }

        public void Delete(int id) => _repo.Delete(id);

        internal static ExerciseDto MapToDto(Exercise e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            EstimatedTime = e.EstimatedTime,
            IsPrintable = e.IsPrintable,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            CreatedById = e.CreatedById,
            CreatedByName = $"{e.CreatedBy.FirstName} {e.CreatedBy.LastName}",
            WorkshopCount = e.Workshops?.Count ?? 0
        };
    }
}
