using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IChildRepository _childRepo;
        private readonly IWorkshopRepository _workshopRepo;
        private readonly IWorkshopAwardService _awardService;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepo,
            IChildRepository childRepo,
            IWorkshopRepository workshopRepo,
            IWorkshopAwardService awardService)
        {
            _enrollmentRepo = enrollmentRepo;
            _childRepo = childRepo;
            _workshopRepo = workshopRepo;
            _awardService = awardService;
        }

        public EnrollmentDto Enroll(int childId, int parentId, CreateEnrollmentDto dto)
        {
            var child = _childRepo.GetByIdForParent(childId, parentId)
                ?? throw new KeyNotFoundException($"Child with id {childId} not found.");

            return CreateEnrollment(child, dto);
        }

        public EnrollmentDto EnrollAsMentor(int childId, CreateEnrollmentDto dto)
        {
            var child = _childRepo.GetById(childId)
                ?? throw new KeyNotFoundException($"Child with id {childId} not found.");

            return CreateEnrollment(child, dto);
        }

        private EnrollmentDto CreateEnrollment(ChildProfile child, CreateEnrollmentDto dto)
        {
            var workshop = _workshopRepo.GetById(dto.WorkshopId)
                ?? throw new ArgumentException($"Workshop with id {dto.WorkshopId} was not found.");

            if (_enrollmentRepo.GetExisting(child.Id, workshop.Id) != null)
                throw new ArgumentException("Child is already enrolled in this workshop.");

            var now = DateTime.UtcNow;
            var enrollment = new Enrollment
            {
                ChildProfileId = child.Id,
                WorkshopId = workshop.Id,
                Status = EnrollmentStatus.Active,
                EnrolledAt = now,
                StatusChangedAt = now
            };

            _enrollmentRepo.Add(enrollment);
            enrollment.Workshop = workshop;
            enrollment.ChildProfile = child;

            return MapToDto(enrollment);
        }

        public List<EnrollmentDto> GetByChild(int childId, int parentId)
        {
            EnsureChildOwned(childId, parentId);
            return _enrollmentRepo.GetByChildForParent(childId, parentId).Select(MapToDto).ToList();
        }

        public EnrollmentDto? GetById(int enrollmentId, int childId, int parentId)
        {
            EnsureChildOwned(childId, parentId);
            var enrollment = _enrollmentRepo.GetByIdForParent(enrollmentId, childId, parentId);
            return enrollment == null ? null : MapToDto(enrollment);
        }

        public EnrollmentDto UpdateStatus(int enrollmentId, int childId, int parentId, UpdateEnrollmentStatusDto dto)
        {
            EnsureChildOwned(childId, parentId);

            if (!Enum.IsDefined(typeof(EnrollmentStatus), dto.Status))
                throw new ArgumentException("Enrollment status value is invalid.");

            var enrollment = _enrollmentRepo.GetByIdForParent(enrollmentId, childId, parentId)
                ?? throw new KeyNotFoundException($"Enrollment with id {enrollmentId} not found.");

            if (enrollment.Status != dto.Status)
            {
                var previous = enrollment.Status;
                enrollment.Status = dto.Status;
                enrollment.StatusChangedAt = DateTime.UtcNow;
                _enrollmentRepo.Update(enrollment);

                if (previous == EnrollmentStatus.Active && dto.Status == EnrollmentStatus.Completed)
                    _awardService.IssueForCompletedEnrollment(enrollment);
            }

            return MapToDto(enrollment);
        }

        private void EnsureChildOwned(int childId, int parentId)
        {
            if (_childRepo.GetByIdForParent(childId, parentId) == null)
                throw new KeyNotFoundException($"Child with id {childId} not found.");
        }

        private static EnrollmentDto MapToDto(Enrollment e) => new()
        {
            Id = e.Id,
            ChildProfileId = e.ChildProfileId,
            WorkshopId = e.WorkshopId,
            WorkshopTitle = e.Workshop?.Title ?? string.Empty,
            Status = e.Status,
            EnrolledAt = e.EnrolledAt,
            StatusChangedAt = e.StatusChangedAt
        };
    }
}
