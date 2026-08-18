using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IActivityProgressService
    {
        List<ActivityProgressDto> GetByEnrollment(int enrollmentId, int childId, int parentId);
        List<ActivityProgressDto> GetByChildAndWorkshop(int childId, int workshopId, int parentId);
        ActivityProgressDto Upsert(int childId, int workshopId, int parentId, UpdateActivityProgressDto dto);
    }
}
