using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IActivityProgressRepository
    {
        List<ActivityProgress> GetByEnrollment(int enrollmentId);
        ActivityProgress? GetByEnrollmentAndUnit(int enrollmentId, ActivityUnitType unitType, int unitId);
        void Add(ActivityProgress progress);
        void Update(ActivityProgress progress);
        bool IsUnitInWorkshop(int workshopId, ActivityUnitType unitType, int unitId);
        List<DateTime> GetDoneTimestampsForChild(int childId);
    }
}
