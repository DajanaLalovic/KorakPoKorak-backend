using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.Services
{
    public class ActivityProgressService : IActivityProgressService
    {
        private readonly IActivityProgressRepository _progressRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IChildRepository _childRepo;
        private readonly IWorkshopRepository _workshopRepo;
        private readonly IWorkshopAwardService _awardService;

        public ActivityProgressService(
            IActivityProgressRepository progressRepo,
            IEnrollmentRepository enrollmentRepo,
            IChildRepository childRepo,
            IWorkshopRepository workshopRepo,
            IWorkshopAwardService awardService)
        {
            _progressRepo = progressRepo;
            _enrollmentRepo = enrollmentRepo;
            _childRepo = childRepo;
            _workshopRepo = workshopRepo;
            _awardService = awardService;
        }

        public List<ActivityProgressDto> GetByEnrollment(int enrollmentId, int childId, int parentId)
        {
            EnsureChildOwned(childId, parentId);

            var enrollment = _enrollmentRepo.GetByIdForParent(enrollmentId, childId, parentId)
                ?? throw new KeyNotFoundException($"Enrollment with id {enrollmentId} not found.");

            return _progressRepo.GetByEnrollment(enrollment.Id)
                .Select(p => MapToDto(p, enrollment))
                .ToList();
        }

        public List<ActivityProgressDto> GetByChildAndWorkshop(int childId, int workshopId, int parentId)
        {
            EnsureChildOwned(childId, parentId);

            var enrollment = _enrollmentRepo.GetByChildAndWorkshopForParent(childId, workshopId, parentId)
                ?? throw new KeyNotFoundException("Enrollment for this child and workshop was not found.");

            return _progressRepo.GetByEnrollment(enrollment.Id)
                .Select(p => MapToDto(p, enrollment))
                .ToList();
        }

        public ActivityProgressDto Upsert(int childId, int workshopId, int parentId, UpdateActivityProgressDto dto)
        {
            EnsureChildOwned(childId, parentId);

            if (!Enum.IsDefined(typeof(ActivityUnitType), dto.UnitType))
                throw new ArgumentException("Unit type value is invalid.");

            if (!Enum.IsDefined(typeof(ActivityProgressStatus), dto.Status))
                throw new ArgumentException("Progress status value is invalid.");

            if (dto.Context.HasValue && !Enum.IsDefined(typeof(ActivityContext), dto.Context.Value))
                throw new ArgumentException("Context value is invalid.");

            if (dto.UnitId <= 0)
                throw new ArgumentException("Unit id is required.");

            var enrollment = _enrollmentRepo.GetByChildAndWorkshopForParent(childId, workshopId, parentId)
                ?? throw new KeyNotFoundException("Enrollment for this child and workshop was not found.");

            if (enrollment.Status == EnrollmentStatus.Withdrawn)
                throw new ArgumentException("Cannot update progress for a withdrawn enrollment.");

            if (!_progressRepo.IsUnitInWorkshop(workshopId, dto.UnitType, dto.UnitId))
                throw new ArgumentException("Lesson/Exercise does not belong to this workshop.");

            var existing = _progressRepo.GetByEnrollmentAndUnit(enrollment.Id, dto.UnitType, dto.UnitId);
            if (existing == null)
            {
                existing = new ActivityProgress
                {
                    EnrollmentId = enrollment.Id,
                    UnitType = dto.UnitType,
                    UnitId = dto.UnitId,
                    Status = dto.Status,
                    Context = dto.Context,
                    LastUpdated = DateTime.UtcNow
                };
                _progressRepo.Add(existing);
            }
            else
            {
                existing.Status = dto.Status;
                existing.Context = dto.Context;
                existing.LastUpdated = DateTime.UtcNow;
                _progressRepo.Update(existing);
            }

            if (dto.Status == ActivityProgressStatus.Done)
                TryCompleteEnrollment(enrollment);

            return MapToDto(existing, enrollment);
        }

        private void TryCompleteEnrollment(Enrollment enrollment)
        {
            if (enrollment.Status != EnrollmentStatus.Active)
                return;

            var lessons = _workshopRepo.GetWorkshopLessons(enrollment.WorkshopId);
            var exercises = _workshopRepo.GetWorkshopExercises(enrollment.WorkshopId);

            var requiredUnits = lessons
                .Select(l => (ActivityUnitType.Lesson, l.Id))
                .Concat(exercises.Select(e => (ActivityUnitType.Exercise, e.Id)))
                .ToList();

            var progressRows = _progressRepo.GetByEnrollment(enrollment.Id)
                .Select(p => (p.UnitType, p.UnitId, p.Status))
                .ToList();

            if (!WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                    enrollment.Status,
                    requiredUnits,
                    progressRows))
                return;

            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.StatusChangedAt = DateTime.UtcNow;
            _enrollmentRepo.Update(enrollment);
            _awardService.IssueForCompletedEnrollment(enrollment);
        }

        private void EnsureChildOwned(int childId, int parentId)
        {
            if (_childRepo.GetByIdForParent(childId, parentId) == null)
                throw new KeyNotFoundException($"Child with id {childId} not found.");
        }

        private static ActivityProgressDto MapToDto(ActivityProgress p, Enrollment enrollment) => new()
        {
            Id = p.Id,
            EnrollmentId = p.EnrollmentId,
            ChildProfileId = enrollment.ChildProfileId,
            WorkshopId = enrollment.WorkshopId,
            UnitType = p.UnitType,
            UnitId = p.UnitId,
            Status = p.Status,
            Context = p.Context,
            LastUpdated = p.LastUpdated
        };
    }
}
