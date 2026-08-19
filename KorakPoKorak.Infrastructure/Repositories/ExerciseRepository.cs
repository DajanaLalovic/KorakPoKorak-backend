using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly AppDbContext _context;

        public ExerciseRepository(AppDbContext context)
        {
            _context = context;
        }

        public (List<Exercise> Items, int Total) GetFiltered(ExerciseQueryParams q)
        {
            var query = _context.Exercises
                .Include(e => e.CreatedBy)
                .Include(e => e.Workshops)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var search = q.Search.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(search));
            }

            if (q.Printable.HasValue)
                query = query.Where(e => e.IsPrintable == q.Printable.Value);

            if (q.CreatedById.HasValue)
                query = query.Where(e => e.CreatedById == q.CreatedById.Value);

            if (q.Status.HasValue)
                query = query.Where(e => e.Status == q.Status.Value);

            var total = query.Count();
            var items = query
                .OrderByDescending(e => e.CreatedAt)
                .Skip(q.Page * q.Size)
                .Take(q.Size)
                .ToList();

            return (items, total);
        }

        public Exercise? GetById(int id)
        {
            return _context.Exercises
                .Include(e => e.CreatedBy)
                .Include(e => e.Workshops)
                .FirstOrDefault(e => e.Id == id);
        }

        public List<Exercise> GetMy(int userId)
        {
            return _context.Exercises
                .Include(e => e.CreatedBy)
                .Include(e => e.Workshops)
                .Where(e => e.CreatedById == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        public List<Exercise> GetRecent(int count)
        {
            return _context.Exercises
                .Include(e => e.CreatedBy)
                .Include(e => e.Workshops)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToList();
        }

        public void Add(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            _context.SaveChanges();
        }

        public void Update(Exercise exercise)
        {
            _context.Exercises.Update(exercise);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var exercise = _context.Exercises.Find(id);
            if (exercise != null)
            {
                _context.Exercises.Remove(exercise);
                _context.SaveChanges();
            }
        }
    }
}
