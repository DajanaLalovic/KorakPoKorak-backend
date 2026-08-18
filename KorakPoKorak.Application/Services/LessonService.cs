using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _repo;

        public LessonService(ILessonRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<LessonDto> GetFiltered(LessonQueryParams q)
        {
            var (items, total) = _repo.GetFiltered(q);
            return new PagedResult<LessonDto>
            {
                Content = items.Select(MapToDto).ToList(),
                TotalElements = total,
                Page = q.Page,
                Size = q.Size
            };
        }

        public LessonDto? GetById(int id)
        {
            var lesson = _repo.GetById(id);
            return lesson == null ? null : MapToDto(lesson);
        }

        public List<LessonDto> GetMy(int userId)
        {
            return _repo.GetMy(userId).Select(MapToDto).ToList();
        }

        public List<LessonDto> GetRecent(int count)
        {
            return _repo.GetRecent(count).Select(MapToDto).ToList();
        }

        public void Create(CreateLessonDto dto, int createdById)
        {
            var lesson = new Lesson
            {
                Title = dto.Title,
                EstimatedTime = dto.EstimatedTime,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById
            };
            _repo.Add(lesson);
        }

        public void Update(int id, UpdateLessonDto dto)
        {
            var lesson = _repo.GetById(id)
                ?? throw new KeyNotFoundException($"Lesson with id {id} not found.");
            lesson.Title = dto.Title;
            lesson.EstimatedTime = dto.EstimatedTime;
            lesson.Status = dto.Status;
            _repo.Update(lesson);
        }

        public void Delete(int id) => _repo.Delete(id);

        internal static LessonDto MapToDto(Lesson l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            EstimatedTime = l.EstimatedTime,
            Status = l.Status,
            CreatedAt = l.CreatedAt,
            CreatedById = l.CreatedById,
            CreatedByName = $"{l.CreatedBy.FirstName} {l.CreatedBy.LastName}"
        };
    }
}
