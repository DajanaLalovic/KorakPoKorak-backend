using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _repo;
        private readonly IContentBlockRepository _contentRepo;
        private readonly IQuizRepository _quizRepo;

        public ExerciseService(
            IExerciseRepository repo,
            IContentBlockRepository contentRepo,
            IQuizRepository quizRepo)
        {
            _repo = repo;
            _contentRepo = contentRepo;
            _quizRepo = quizRepo;
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

        public ExerciseDto Create(CreateExerciseDto dto, int createdById)
        {
            EnsureQuizExists(dto.QuizId);

            var exercise = new Exercise
            {
                Title = dto.Title,
                EstimatedTime = dto.EstimatedTime,
                IsPrintable = dto.IsPrintable,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedById = createdById,
                QuizId = dto.QuizId
            };
            _repo.Add(exercise);

            if (dto.ContentBlocks != null && dto.ContentBlocks.Count > 0)
            {
                var assets = ContentBlockMapper.BuildAssets(dto.ContentBlocks, createdById);
                _contentRepo.ReplaceForExercise(exercise.Id, assets);
            }

            return MapToDto(_repo.GetById(exercise.Id)!);
        }

        public ExerciseDto Update(int id, UpdateExerciseDto dto)
        {
            var exercise = _repo.GetById(id)
                ?? throw new KeyNotFoundException($"Exercise with id {id} not found.");

            EnsureQuizExists(dto.QuizId);

            exercise.Title = dto.Title;
            exercise.EstimatedTime = dto.EstimatedTime;
            exercise.IsPrintable = dto.IsPrintable;
            exercise.Status = dto.Status;
            exercise.QuizId = dto.QuizId;
            _repo.Update(exercise);

            if (dto.ContentBlocks != null)
            {
                IReadOnlyList<(MediaAsset Asset, int OrderIndex)> assets = dto.ContentBlocks.Count == 0
                    ? Array.Empty<(MediaAsset Asset, int OrderIndex)>()
                    : ContentBlockMapper.BuildAssets(dto.ContentBlocks, exercise.CreatedById);
                _contentRepo.ReplaceForExercise(exercise.Id, assets);
            }

            return MapToDto(_repo.GetById(id)!);
        }

        public void Delete(int id) => _repo.Delete(id);

        private void EnsureQuizExists(int? quizId)
        {
            if (!quizId.HasValue) return;
            if (_quizRepo.GetById(quizId.Value) == null)
                throw new ArgumentException($"Quiz with id {quizId.Value} was not found.");
        }

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
            WorkshopCount = e.Workshops?.Count ?? 0,
            ContentBlocks = ContentBlockMapper.ToDtoList(e.ContentBlocks),
            QuizId = e.QuizId,
            Quiz = e.Quiz == null ? null : QuizService.MapToDto(e.Quiz)
        };
    }
}
