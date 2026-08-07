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
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.ToLower();
                query = query.Where(qz => qz.Title.ToLower().Contains(s));
            }

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
                .FirstOrDefault(qz => qz.Id == id);
        }

        public List<Quiz> GetMy(int userId)
        {
            return _context.Quizzes
                .Include(qz => qz.CreatedBy)
                .Include(qz => qz.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Where(qz => qz.CreatedById == userId)
                .OrderByDescending(qz => qz.CreatedAt)
                .ToList();
        }

        public void Add(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            _context.SaveChanges();
        }

        public void Update(Quiz quiz)
        {
            // Remove existing questions/answers and replace with new ones (simplest approach)
            var existing = _context.Quizzes
                .Include(qz => qz.Questions)
                    .ThenInclude(qq => qq.Answers)
                .First(qz => qz.Id == quiz.Id);

            existing.Title = quiz.Title;

            // Delete old questions (cascade deletes answers)
            _context.Questions.RemoveRange(existing.Questions);

            // Add new questions
            foreach (var q in quiz.Questions)
            {
                q.QuizId = existing.Id;
                _context.Questions.Add(q);
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
