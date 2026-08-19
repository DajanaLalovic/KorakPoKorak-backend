using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class WorkshopRepository : IWorkshopRepository
    {
        private readonly AppDbContext _context;

        public WorkshopRepository(AppDbContext context)
        {
            _context = context;
        }

        public (List<Workshop> Items, int Total) GetFiltered(WorkshopQueryParams q)
        {
            var query = _context.Workshops
                .Include(w => w.Lessons)
                .Include(w => w.Exercises)
                .Include(w => w.CreatedBy)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var search = q.Search.ToLower();
                query = query.Where(w => w.Title.ToLower().Contains(search)
                                      || w.Description.ToLower().Contains(search));
            }

            if (q.Status.HasValue)
                query = query.Where(w => w.Status == q.Status.Value);

            if (q.Complexity.HasValue)
                query = query.Where(w => w.ComplexityLevel == q.Complexity.Value);

            var total = query.Count();
            var items = query
                .OrderByDescending(w => w.CreatedAt)
                .Skip(q.Page * q.Size)
                .Take(q.Size)
                .ToList();

            return (items, total);
        }

        public (int Total, int Published, int Draft) GetCounts()
        {
            var total = _context.Workshops.Count();
            var published = _context.Workshops.Count(w => w.Status == WorkshopStatus.Published);
            var draft = _context.Workshops.Count(w => w.Status == WorkshopStatus.Draft);
            return (total, published, draft);
        }

        public Workshop? GetById(int id)
        {
            return _context.Workshops
                .Include(w => w.Lessons)
                .Include(w => w.Exercises)
                .Include(w => w.CreatedBy)
                .FirstOrDefault(w => w.Id == id);
        }

        public List<Workshop> GetMy(int userId)
        {
            return _context.Workshops
                .Include(w => w.Lessons)
                .Include(w => w.Exercises)
                .Include(w => w.CreatedBy)
                .Where(w => w.CreatedById == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToList();
        }

        public List<Workshop> GetRecent(int count)
        {
            return _context.Workshops
                .Include(w => w.Lessons)
                .Include(w => w.Exercises)
                .Include(w => w.CreatedBy)
                .OrderByDescending(w => w.CreatedAt)
                .Take(count)
                .ToList();
        }

        public List<Lesson> GetWorkshopLessons(int workshopId)
        {
            return _context.Lessons
                .Include(l => l.CreatedBy)
                .Include(l => l.ContentBlocks)
                    .ThenInclude(b => b.MediaAsset)
                .Where(l => l.Workshops.Any(w => w.Id == workshopId))
                .ToList();
        }

        public List<Exercise> GetWorkshopExercises(int workshopId)
        {
            return _context.Exercises
                .Include(e => e.CreatedBy)
                .Include(e => e.ContentBlocks)
                    .ThenInclude(b => b.MediaAsset)
                .Where(e => e.Workshops.Any(w => w.Id == workshopId))
                .ToList();
        }

        public void Add(Workshop workshop, List<int> lessonIds, List<int> exerciseIds)
        {
            if (lessonIds.Count > 0)
                workshop.Lessons = _context.Lessons.Where(l => lessonIds.Contains(l.Id)).ToList();

            if (exerciseIds.Count > 0)
                workshop.Exercises = _context.Exercises.Where(e => exerciseIds.Contains(e.Id)).ToList();

            _context.Workshops.Add(workshop);
            _context.SaveChanges();
        }

        public void Update(Workshop workshop, List<int> lessonIds, List<int> exerciseIds)
        {
            var existing = _context.Workshops
                .Include(w => w.Lessons)
                .Include(w => w.Exercises)
                .FirstOrDefault(w => w.Id == workshop.Id);

            if (existing == null) return;

            existing.Title = workshop.Title;
            existing.Description = workshop.Description;
            existing.AgeMin = workshop.AgeMin;
            existing.AgeMax = workshop.AgeMax;
            existing.ComplexityLevel = workshop.ComplexityLevel;
            existing.ActivityTypes = workshop.ActivityTypes;
            existing.Status = workshop.Status;

            existing.Lessons = _context.Lessons.Where(l => lessonIds.Contains(l.Id)).ToList();
            existing.Exercises = _context.Exercises.Where(e => exerciseIds.Contains(e.Id)).ToList();

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var workshop = _context.Workshops.Find(id);
            if (workshop != null)
            {
                _context.Workshops.Remove(workshop);
                _context.SaveChanges();
            }
        }

        public void PatchStatus(int id, WorkshopStatus status)
        {
            var workshop = _context.Workshops.Find(id)
                ?? throw new KeyNotFoundException($"Workshop with id {id} not found.");

            workshop.Status = status;
            _context.SaveChanges();
        }

        public void AttachLesson(int workshopId, int lessonId)
        {
            var workshop = _context.Workshops
                .Include(w => w.Lessons)
                .FirstOrDefault(w => w.Id == workshopId)
                ?? throw new KeyNotFoundException($"Workshop with id {workshopId} not found.");

            var lesson = _context.Lessons.Find(lessonId)
                ?? throw new KeyNotFoundException($"Lesson with id {lessonId} not found.");

            if (!workshop.Lessons.Any(l => l.Id == lessonId))
            {
                workshop.Lessons.Add(lesson);
                _context.SaveChanges();
            }
        }

        public void DetachLesson(int workshopId, int lessonId)
        {
            var workshop = _context.Workshops
                .Include(w => w.Lessons)
                .FirstOrDefault(w => w.Id == workshopId);

            if (workshop == null) return;

            var lesson = workshop.Lessons.FirstOrDefault(l => l.Id == lessonId);
            if (lesson != null)
            {
                workshop.Lessons.Remove(lesson);
                _context.SaveChanges();
            }
        }

        public void AttachExercise(int workshopId, int exerciseId)
        {
            var workshop = _context.Workshops
                .Include(w => w.Exercises)
                .FirstOrDefault(w => w.Id == workshopId)
                ?? throw new KeyNotFoundException($"Workshop with id {workshopId} not found.");

            var exercise = _context.Exercises.Find(exerciseId)
                ?? throw new KeyNotFoundException($"Exercise with id {exerciseId} not found.");

            if (!workshop.Exercises.Any(e => e.Id == exerciseId))
            {
                workshop.Exercises.Add(exercise);
                _context.SaveChanges();
            }
        }

        public void DetachExercise(int workshopId, int exerciseId)
        {
            var workshop = _context.Workshops
                .Include(w => w.Exercises)
                .FirstOrDefault(w => w.Id == workshopId);

            if (workshop == null) return;

            var exercise = workshop.Exercises.FirstOrDefault(e => e.Id == exerciseId);
            if (exercise != null)
            {
                workshop.Exercises.Remove(exercise);
                _context.SaveChanges();
            }
        }
    }
}
