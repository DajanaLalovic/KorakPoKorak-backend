using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class WorkshopAwardService : IWorkshopAwardService
    {
        private readonly IBadgeRepository _badgeRepo;
        private readonly ICertificateRepository _certificateRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IWorkshopRepository _workshopRepo;
        private readonly IChildRepository _childRepo;

        public WorkshopAwardService(
            IBadgeRepository badgeRepo,
            ICertificateRepository certificateRepo,
            IEnrollmentRepository enrollmentRepo,
            IWorkshopRepository workshopRepo,
            IChildRepository childRepo)
        {
            _badgeRepo = badgeRepo;
            _certificateRepo = certificateRepo;
            _enrollmentRepo = enrollmentRepo;
            _workshopRepo = workshopRepo;
            _childRepo = childRepo;
        }

        public void IssueForCompletedEnrollment(Enrollment enrollment)
        {
            if (enrollment.Status != EnrollmentStatus.Completed)
                return;

            var workshop = ResolveWorkshop(enrollment);
            var completedCount = _enrollmentRepo.CountCompletedByChild(enrollment.ChildProfileId);
            var activityTypeCount = workshop.ActivityTypes?.Count ?? 0;

            var codes = WorkshopAwardRules.GetBadgeCodes(
                completedCount,
                workshop.ComplexityLevel,
                activityTypeCount);

            foreach (var code in codes)
                TryAwardBadge(code, enrollment, workshop);

            TryIssueCertificate(enrollment, workshop);
        }

        public List<BadgeAwardDto> GetBadgesForChild(int childId, int parentId)
        {
            EnsureChildOwned(childId, parentId);
            return _badgeRepo.GetAwardsForChild(childId).Select(MapBadge).ToList();
        }

        public List<CertificateAwardDto> GetCertificatesForChild(int childId, int parentId)
        {
            EnsureChildOwned(childId, parentId);
            return _certificateRepo.GetAwardsForChild(childId).Select(MapCertificate).ToList();
        }

        public CertificateAwardDto? GetCertificateById(int certificateAwardId, int childId, int parentId)
        {
            EnsureChildOwned(childId, parentId);
            var award = _certificateRepo.GetAwardByIdForParent(certificateAwardId, childId, parentId);
            return award == null ? null : MapCertificate(award);
        }

        private void TryAwardBadge(string code, Enrollment enrollment, Workshop workshop)
        {
            var template = _badgeRepo.GetTemplateByCode(code);
            if (template == null)
                return;

            if (template.Scope == BadgeAwardScope.OncePerChild)
            {
                if (_badgeRepo.ExistsForChild(enrollment.ChildProfileId, template.Id))
                    return;
            }
            else if (_badgeRepo.ExistsForChildWorkshop(enrollment.ChildProfileId, template.Id, workshop.Id))
            {
                return;
            }

            _badgeRepo.AddAward(new BadgeAward
            {
                BadgeTemplateId = template.Id,
                ChildProfileId = enrollment.ChildProfileId,
                WorkshopId = workshop.Id,
                EnrollmentId = enrollment.Id,
                Scope = template.Scope,
                AwardedAt = DateTime.UtcNow
            });
        }

        private void TryIssueCertificate(Enrollment enrollment, Workshop workshop)
        {
            if (_certificateRepo.ExistsForEnrollment(enrollment.Id))
                return;

            var template = _certificateRepo.GetDefaultTemplate();
            if (template == null)
                return;

            var child = enrollment.ChildProfile;
            var childName = child == null
                ? string.Empty
                : $"{child.FirstName} {child.LastName}".Trim();

            var mentor = workshop.CreatedBy;
            var mentorName = mentor == null
                ? string.Empty
                : $"{mentor.FirstName} {mentor.LastName}".Trim();

            _certificateRepo.AddAward(new CertificateAward
            {
                CertificateTemplateId = template.Id,
                ChildProfileId = enrollment.ChildProfileId,
                WorkshopId = workshop.Id,
                EnrollmentId = enrollment.Id,
                ChildFullName = childName,
                WorkshopTitle = workshop.Title,
                MentorName = mentorName,
                CertificateTitle = template.Title,
                CertificateNumber = $"KPK-{enrollment.Id:D6}",
                IssuedAt = DateTime.UtcNow
            });
        }

        private Workshop ResolveWorkshop(Enrollment enrollment)
        {
            if (enrollment.Workshop?.CreatedBy != null)
                return enrollment.Workshop;

            return _workshopRepo.GetById(enrollment.WorkshopId)
                ?? enrollment.Workshop
                ?? throw new KeyNotFoundException($"Workshop with id {enrollment.WorkshopId} was not found.");
        }

        private void EnsureChildOwned(int childId, int parentId)
        {
            if (_childRepo.GetByIdForParent(childId, parentId) == null)
                throw new KeyNotFoundException($"Child with id {childId} not found.");
        }

        private static BadgeAwardDto MapBadge(BadgeAward a) => new()
        {
            Id = a.Id,
            BadgeTemplateId = a.BadgeTemplateId,
            Code = a.BadgeTemplate?.Code ?? string.Empty,
            Name = a.BadgeTemplate?.Name ?? string.Empty,
            Description = a.BadgeTemplate?.Description ?? string.Empty,
            IconUrl = a.BadgeTemplate?.IconUrl ?? string.Empty,
            Category = a.BadgeTemplate?.Category ?? default,
            ChildProfileId = a.ChildProfileId,
            WorkshopId = a.WorkshopId,
            WorkshopTitle = a.Workshop?.Title,
            EnrollmentId = a.EnrollmentId,
            AwardedAt = a.AwardedAt
        };

        private static CertificateAwardDto MapCertificate(CertificateAward a) => new()
        {
            Id = a.Id,
            CertificateTemplateId = a.CertificateTemplateId,
            Title = string.IsNullOrWhiteSpace(a.CertificateTitle)
                ? a.CertificateTemplate?.Title ?? string.Empty
                : a.CertificateTitle,
            Description = a.CertificateTemplate?.Description ?? string.Empty,
            ChildProfileId = a.ChildProfileId,
            ChildFullName = a.ChildFullName,
            WorkshopId = a.WorkshopId,
            WorkshopTitle = a.WorkshopTitle,
            EnrollmentId = a.EnrollmentId,
            MentorName = a.MentorName,
            CertificateNumber = a.CertificateNumber,
            IssuedAt = a.IssuedAt
        };
    }
}
