using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AppDbContext _context;

        public CertificateRepository(AppDbContext context)
        {
            _context = context;
        }

        public CertificateTemplate? GetDefaultTemplate()
        {
            return _context.CertificateTemplates
                .FirstOrDefault(t => t.Code == CertificateCodes.DefaultWorkshop);
        }

        public bool ExistsForEnrollment(int enrollmentId)
        {
            return _context.CertificateAwards.Any(a => a.EnrollmentId == enrollmentId);
        }

        public void AddAward(CertificateAward award)
        {
            _context.CertificateAwards.Add(award);
            _context.SaveChanges();
        }

        public List<CertificateAward> GetAwardsForChild(int childId)
        {
            return _context.CertificateAwards
                .Include(a => a.CertificateTemplate)
                .Include(a => a.Workshop)
                .Where(a => a.ChildProfileId == childId)
                .OrderByDescending(a => a.IssuedAt)
                .ToList();
        }

        public CertificateAward? GetAwardByIdForParent(int awardId, int childId, int parentId)
        {
            return _context.CertificateAwards
                .Include(a => a.CertificateTemplate)
                .Include(a => a.Workshop)
                .Include(a => a.ChildProfile)
                .FirstOrDefault(a =>
                    a.Id == awardId
                    && a.ChildProfileId == childId
                    && a.ChildProfile.ParentId == parentId);
        }
    }
}
