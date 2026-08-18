using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IEnrollmentRepository
    {
        List<Enrollment> GetByChildForParent(int childId, int parentId);
        Enrollment? GetByIdForParent(int enrollmentId, int childId, int parentId);
        Enrollment? GetByChildAndWorkshopForParent(int childId, int workshopId, int parentId);
        Enrollment? GetExisting(int childId, int workshopId);
        void Add(Enrollment enrollment);
        void Update(Enrollment enrollment);
    }
}
