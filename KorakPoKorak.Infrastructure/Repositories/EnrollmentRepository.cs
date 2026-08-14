using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Enrollment> GetByChildForParent(int childId, int parentId)
        {
            return _context.Enrollments
                .Include(e => e.Workshop)
                .Include(e => e.ChildProfile)
                .Where(e => e.ChildProfileId == childId && e.ChildProfile.ParentId == parentId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToList();
        }

        public Enrollment? GetByIdForParent(int enrollmentId, int childId, int parentId)
        {
            return _context.Enrollments
                .Include(e => e.Workshop)
                .Include(e => e.ChildProfile)
                .FirstOrDefault(e =>
                    e.Id == enrollmentId
                    && e.ChildProfileId == childId
                    && e.ChildProfile.ParentId == parentId);
        }

        public Enrollment? GetByChildAndWorkshopForParent(int childId, int workshopId, int parentId)
        {
            return _context.Enrollments
                .Include(e => e.Workshop)
                .Include(e => e.ChildProfile)
                .FirstOrDefault(e =>
                    e.ChildProfileId == childId
                    && e.WorkshopId == workshopId
                    && e.ChildProfile.ParentId == parentId);
        }

        public Enrollment? GetExisting(int childId, int workshopId)
        {
            return _context.Enrollments
                .FirstOrDefault(e => e.ChildProfileId == childId && e.WorkshopId == workshopId);
        }

        public void Add(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
        }

        public void Update(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            _context.SaveChanges();
        }
    }
}
