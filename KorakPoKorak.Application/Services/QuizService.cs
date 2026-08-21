using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _repo;

        public QuizService(IQuizRepository repo)
        {
            _repo = repo;
        }

        public PagedResult<QuizDto> GetFiltered(QuizQueryParams q)
        {
            var (items, total) = _repo.GetFiltered(q);
            return new PagedResult<QuizDto>
            {
                Content       = items.Select(MapToDto).ToList(),
                TotalElements = total,
                Page          = q.Page,
                Size          = q.Size
            };
        }

        public QuizDto? GetById(int id)
        {
            var quiz = _repo.GetById(id);
            return quiz == null ? null : MapToDto(quiz);
        }

        public List<QuizDto> GetMy(int userId)
        {
            return _repo.GetMy(userId).Select(MapToDto).ToList();
        }

        public QuizDto Create(CreateQuizDto dto, int createdById)
        {
            var quiz = new Quiz
            {
                Title       = dto.Title,
                CreatedAt   = DateTime.UtcNow,
                CreatedById = createdById,
                Questions   = dto.Questions.Select((qDto, i) => new Question
                {
                    Text       = qDto.Text,
                    Type       = ParseType(qDto.Type),
                    Points     = qDto.Points > 0 ? qDto.Points : 1,
                    OrderIndex = qDto.OrderIndex > 0 ? qDto.OrderIndex : i,
                    Answers    = qDto.Answers.Select((aDto, j) => new Answer
                    {
                        Text       = aDto.Text,
                        IsCorrect  = aDto.IsCorrect,
                        OrderIndex = aDto.OrderIndex > 0 ? aDto.OrderIndex : j
                    }).ToList()
                }).ToList()
            };

            _repo.Add(quiz);
            return MapToDto(quiz);
        }

        public QuizDto Update(int id, UpdateQuizDto dto)
        {
            var incoming = new Quiz
            {
                Id = id,
                Title = dto.Title,
                Questions = dto.Questions.Select((qDto, i) => new Question
                {
                    Text       = qDto.Text,
                    Type       = ParseType(qDto.Type),
                    Points     = qDto.Points > 0 ? qDto.Points : 1,
                    OrderIndex = qDto.OrderIndex > 0 ? qDto.OrderIndex : i,
                    Answers    = qDto.Answers.Select((aDto, j) => new Answer
                    {
                        Text       = aDto.Text,
                        IsCorrect  = aDto.IsCorrect,
                        OrderIndex = aDto.OrderIndex > 0 ? aDto.OrderIndex : j
                    }).ToList()
                }).ToList()
            };

            _repo.Update(incoming);
            return MapToDto(_repo.GetById(id)!);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static QuestionType ParseType(string type) => type switch
        {
            "MULTI_CHOICE" => QuestionType.MULTI_CHOICE,
            "TRUE_FALSE"   => QuestionType.TRUE_FALSE,
            _              => QuestionType.SINGLE_CHOICE
        };

        internal static QuizDto MapToDto(Quiz q) => new()
        {
            Id            = q.Id,
            Title         = q.Title,
            CreatedAt     = q.CreatedAt,
            CreatedById   = q.CreatedById,
            CreatedByName = q.CreatedBy != null
                ? $"{q.CreatedBy.FirstName} {q.CreatedBy.LastName}"
                : string.Empty,
            QuestionCount = q.Questions?.Count ?? 0,
            ExerciseCount = q.Exercises?.Count ?? 0,
            Questions     = (q.Questions ?? [])
                .OrderBy(qq => qq.OrderIndex)
                .Select(qq => new QuestionDto
                {
                    Id         = qq.Id,
                    Text       = qq.Text,
                    Type       = qq.Type.ToString(),
                    Points     = qq.Points,
                    OrderIndex = qq.OrderIndex,
                    Answers    = (qq.Answers ?? [])
                        .OrderBy(a => a.OrderIndex)
                        .Select(a => new AnswerDto
                        {
                            Id         = a.Id,
                            Text       = a.Text,
                            IsCorrect  = a.IsCorrect,
                            OrderIndex = a.OrderIndex
                        }).ToList()
                }).ToList()
        };
    }
}
