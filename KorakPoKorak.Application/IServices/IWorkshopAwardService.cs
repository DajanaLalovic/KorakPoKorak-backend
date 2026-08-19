using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IServices
{
    public interface IWorkshopAwardService
    {
        void IssueForCompletedEnrollment(Enrollment enrollment);
        List<BadgeAwardDto> GetBadgesForChild(int childId, int parentId);
        List<CertificateAwardDto> GetCertificatesForChild(int childId, int parentId);
        CertificateAwardDto? GetCertificateById(int certificateAwardId, int childId, int parentId);
    }
}
