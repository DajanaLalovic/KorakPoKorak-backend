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

        private IQueryable<Lesson> LessonsWithContent() =>
            _context.Lessons
                .Include(l => l.CreatedBy)
                .Include(l => l.ContentBlocks)
                    .ThenInclude(b => b.MediaAsset);

        public (List<Lesson> Items, int Total) GetFiltered(LessonQueryParams q)
        {
            var query = LessonsWithContent().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var search = q.Search.ToLower();
                query = query.Where(l => l.Title.ToLower().Contains(search));
            }

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
            return LessonsWithContent().FirstOrDefault(l => l.Id == id);
        }

        public List<Lesson> GetMy(int userId)
        {
            return LessonsWithContent()
                .Where(l => l.CreatedById == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToList();
        }

        public List<Lesson> GetRecent(int count)
        {
            return LessonsWithContent()
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
            var lesson = LessonsWithContent().FirstOrDefault(l => l.Id == id);
            if (lesson == null)
                return;

            var assets = lesson.ContentBlocks.Select(b => b.MediaAsset).ToList();
            if (lesson.ContentBlocks.Count > 0)
            {
                _context.ContentBlocks.RemoveRange(lesson.ContentBlocks);
                _context.MediaAssets.RemoveRange(assets);
            }

            _context.Lessons.Remove(lesson);
            _context.SaveChanges();
        }
    }
}
