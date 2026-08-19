using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Tests
{
    public class ChildStatsRulesTests
    {
        private static readonly DateOnly Today = new(2026, 8, 17);

        [Fact]
        public void ThreeConsecutiveDaysEndingToday_ReturnsThree()
        {
            var dates = new[]
            {
                new DateOnly(2026, 8, 15),
                new DateOnly(2026, 8, 16),
                new DateOnly(2026, 8, 17)
            };

            Assert.Equal(3, ChildStatsRules.CountStreakDays(dates, Today));
        }

        [Fact]
        public void ConsecutiveDaysEndingYesterday_StillCounts()
        {
            var dates = new[]
            {
                new DateOnly(2026, 8, 15),
                new DateOnly(2026, 8, 16)
            };

            Assert.Equal(2, ChildStatsRules.CountStreakDays(dates, Today));
        }

        [Fact]
        public void LastCompletionOlderThanYesterday_ReturnsZero()
        {
            var dates = new[] { new DateOnly(2026, 8, 15) };

            Assert.Equal(0, ChildStatsRules.CountStreakDays(dates, Today));
        }

        [Fact]
        public void GapBreaksStreak()
        {
            var dates = new[]
            {
                new DateOnly(2026, 8, 13),
                new DateOnly(2026, 8, 17)
            };

            Assert.Equal(1, ChildStatsRules.CountStreakDays(dates, Today));
        }

        [Fact]
        public void Empty_ReturnsZero()
        {
            Assert.Equal(0, ChildStatsRules.CountStreakDays(Array.Empty<DateOnly>(), Today));
        }

        [Fact]
        public void DuplicateDates_CountOnce()
        {
            var dates = new[]
            {
                new DateOnly(2026, 8, 16),
                new DateOnly(2026, 8, 16),
                new DateOnly(2026, 8, 17)
            };

            Assert.Equal(2, ChildStatsRules.CountStreakDays(dates, Today));
        }
    }
}
