using KorakPoKorak.Application.DTOs;
using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Application.IServices;
using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.Services
{
    public class ChildStatsService : IChildStatsService
    {
        private readonly IChildRepository _childRepo;
        private readonly IActivityProgressRepository _progressRepo;

        public ChildStatsService(IChildRepository childRepo, IActivityProgressRepository progressRepo)
        {
            _childRepo = childRepo;
            _progressRepo = progressRepo;
        }

        public ChildStatsDto GetStats(int childId, int parentId)
        {
            if (_childRepo.GetByIdForParent(childId, parentId) == null)
                throw new KeyNotFoundException($"Child with id {childId} not found.");

            var completedAt = _progressRepo.GetDoneTimestampsForChild(childId);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var completionDates = completedAt
                .Select(ts => DateOnly.FromDateTime(ts))
                .ToList();

            return new ChildStatsDto
            {
                ChildProfileId = childId,
                TotalStars = completedAt.Count,
                StreakDays = ChildStatsRules.CountStreakDays(completionDates, today)
            };
        }
    }
}
