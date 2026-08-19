using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface ICertificateRepository
    {
        CertificateTemplate? GetDefaultTemplate();
        bool ExistsForEnrollment(int enrollmentId);
        void AddAward(CertificateAward award);
        List<CertificateAward> GetAwardsForChild(int childId);
        CertificateAward? GetAwardByIdForParent(int awardId, int childId, int parentId);
    }
}
