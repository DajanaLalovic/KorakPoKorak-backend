using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class ActivityProgressRepository : IActivityProgressRepository
    {
        private readonly AppDbContext _context;

        public ActivityProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<ActivityProgress> GetByEnrollment(int enrollmentId)
        {
            return _context.ActivityProgresses
                .Include(p => p.Enrollment)
                .Where(p => p.EnrollmentId == enrollmentId)
                .OrderBy(p => p.UnitType)
                .ThenBy(p => p.UnitId)
                .ToList();
        }

        public ActivityProgress? GetByEnrollmentAndUnit(int enrollmentId, ActivityUnitType unitType, int unitId)
        {
            return _context.ActivityProgresses
                .FirstOrDefault(p =>
                    p.EnrollmentId == enrollmentId
                    && p.UnitType == unitType
                    && p.UnitId == unitId);
        }

        public void Add(ActivityProgress progress)
        {
            _context.ActivityProgresses.Add(progress);
            _context.SaveChanges();
        }

        public void Update(ActivityProgress progress)
        {
            _context.ActivityProgresses.Update(progress);
            _context.SaveChanges();
        }

        public bool IsUnitInWorkshop(int workshopId, ActivityUnitType unitType, int unitId)
        {
            return unitType switch
            {
                ActivityUnitType.Lesson => _context.Lessons
                    .Any(l => l.Id == unitId && l.Workshops.Any(w => w.Id == workshopId)),
                ActivityUnitType.Exercise => _context.Exercises
                    .Any(e => e.Id == unitId && e.Workshops.Any(w => w.Id == workshopId)),
                _ => false
            };
        }
    }
}
