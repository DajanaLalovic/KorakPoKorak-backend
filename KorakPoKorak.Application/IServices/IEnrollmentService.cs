using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.IServices
{
    public interface IEnrollmentService
    {
        EnrollmentDto Enroll(int childId, int parentId, CreateEnrollmentDto dto);
        EnrollmentDto EnrollAsMentor(int childId, CreateEnrollmentDto dto);
        List<EnrollmentDto> GetByChild(int childId, int parentId);
        EnrollmentDto? GetById(int enrollmentId, int childId, int parentId);
        EnrollmentDto UpdateStatus(int enrollmentId, int childId, int parentId, UpdateEnrollmentStatusDto dto);
    }
}
