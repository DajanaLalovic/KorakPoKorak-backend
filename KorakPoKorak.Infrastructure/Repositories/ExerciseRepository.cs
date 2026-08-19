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

        private IQueryable<Exercise> ExercisesWithContent() =>
            _context.Exercises
                .Include(e => e.CreatedBy)
                .Include(e => e.ContentBlocks)
                    .ThenInclude(b => b.MediaAsset);

        public (List<Exercise> Items, int Total) GetFiltered(ExerciseQueryParams q)
        {
            var query = ExercisesWithContent().AsQueryable();

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
            return ExercisesWithContent().FirstOrDefault(e => e.Id == id);
        }

        public List<Exercise> GetMy(int userId)
        {
            return ExercisesWithContent()
                .Where(e => e.CreatedById == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        public List<Exercise> GetRecent(int count)
        {
            return ExercisesWithContent()
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
            var exercise = ExercisesWithContent().FirstOrDefault(e => e.Id == id);
            if (exercise == null)
                return;

            var assets = exercise.ContentBlocks.Select(b => b.MediaAsset).ToList();
            if (exercise.ContentBlocks.Count > 0)
            {
                _context.ContentBlocks.RemoveRange(exercise.ContentBlocks);
                _context.MediaAssets.RemoveRange(assets);
            }

            _context.Exercises.Remove(exercise);
            _context.SaveChanges();
        }
    }
}
