namespace KorakPoKorak.Domain
{
    public static class ChildStatsRules
    {
        /// <summary>
        /// Consecutive calendar days with at least one completed activity,
        /// ending today or yesterday. Zero if the last completion is older than yesterday.
        /// </summary>
        public static int CountStreakDays(IReadOnlyCollection<DateOnly> completionDates, DateOnly today)
        {
            if (completionDates.Count == 0)
                return 0;

            var days = completionDates.ToHashSet();
            var yesterday = today.AddDays(-1);

            DateOnly cursor;
            if (days.Contains(today))
                cursor = today;
            else if (days.Contains(yesterday))
                cursor = yesterday;
            else
                return 0;

            var streak = 0;
            while (days.Contains(cursor))
            {
                streak++;
                cursor = cursor.AddDays(-1);
            }

            return streak;
        }
    }
}
