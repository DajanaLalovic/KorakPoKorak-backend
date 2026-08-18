using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class LessonRepository : ILessonRepository
    {
        private readonly AppDbContext _context;

        public LessonRepository(AppDbContext context)
        {
            _context = context;
        }

        public (List<Lesson> Items, int Total) GetFiltered(LessonQueryParams q)
        {
            var query = _context.Lessons
                .Include(l => l.CreatedBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var search = q.Search.ToLower();
                query = query.Where(l => l.Title.ToLower().Contains(search));
            }

            if (q.CreatedById.HasValue)
                query = query.Where(l => l.CreatedById == q.CreatedById.Value);

            if (q.Status.HasValue)
                query = query.Where(l => l.Status == q.Status.Value);

            var total = query.Count();
            var items = query
                .OrderByDescending(l => l.CreatedAt)
                .Skip(q.Page * q.Size)
                .Take(q.Size)
                .ToList();

            return (items, total);
        }

        public Lesson? GetById(int id)
        {
            return _context.Lessons.Include(l => l.CreatedBy).FirstOrDefault(l => l.Id == id);
        }

        public List<Lesson> GetMy(int userId)
        {
            return _context.Lessons
                .Include(l => l.CreatedBy)
                .Where(l => l.CreatedById == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();
        }

        public List<Lesson> GetRecent(int count)
        {
            return _context.Lessons
                .Include(l => l.CreatedBy)
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToList();
        }

        public void Add(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            _context.SaveChanges();
        }

        public void Update(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                _context.SaveChanges();
            }
        }
    }
}
