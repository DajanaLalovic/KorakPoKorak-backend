using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly AppDbContext _context;

        public QuizRepository(AppDbContext context)
        {
            _context = context;
        }

        public (List<Quiz> Items, int Total) GetFiltered(QuizQueryParams q)
        {
            var query = _context.Quizzes
                .Include(qz => qz.CreatedBy)
                .Include(qz => qz.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Include(qz => qz.Exercises)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.ToLower();
                query = query.Where(qz => qz.Title.ToLower().Contains(s));
            }

            if (q.CreatedById.HasValue)
                query = query.Where(qz => qz.CreatedById == q.CreatedById.Value);

            var total = query.Count();
            var items = query
                .OrderByDescending(qz => qz.CreatedAt)
                .Skip(q.Page * q.Size)
                .Take(q.Size)
                .ToList();

            return (items, total);
        }

        public Quiz? GetById(int id)
        {
            return _context.Quizzes
                .Include(qz => qz.CreatedBy)
                .Include(qz => qz.Questions.OrderBy(qq => qq.OrderIndex))
                    .ThenInclude(qq => qq.Answers.OrderBy(a => a.OrderIndex))
                .Include(qz => qz.Exercises)
                .FirstOrDefault(qz => qz.Id == id);
        }

        public List<Quiz> GetMy(int userId)
        {
            return _context.Quizzes
                .Include(qz => qz.CreatedBy)
                .Include(qz => qz.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Include(qz => qz.Exercises)
                .Where(qz => qz.CreatedById == userId)
                .OrderByDescending(qz => qz.CreatedAt)
                .ToList();
        }

        public void Add(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            _context.SaveChanges();
        }

        public void Update(Quiz incoming)
        {
            var existing = _context.Quizzes
                .Include(qz => qz.CreatedBy)
                .Include(qz => qz.Exercises)
                .Include(qz => qz.Questions)
                    .ThenInclude(qq => qq.Answers)
                .FirstOrDefault(qz => qz.Id == incoming.Id)
                ?? throw new KeyNotFoundException($"Quiz with id {incoming.Id} not found.");

            existing.Title = incoming.Title;

            var oldQuestions = existing.Questions.ToList();
            _context.Questions.RemoveRange(oldQuestions);
            existing.Questions.Clear();

            foreach (var question in incoming.Questions)
            {
                question.Id = 0;
                question.QuizId = existing.Id;
                foreach (var answer in question.Answers)
                    answer.Id = 0;
                existing.Questions.Add(question);
            }

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var quiz = _context.Quizzes
                .Include(qz => qz.Questions)
                    .ThenInclude(qq => qq.Answers)
                .FirstOrDefault(qz => qz.Id == id);

            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                _context.SaveChanges();
            }
        }
    }
}
