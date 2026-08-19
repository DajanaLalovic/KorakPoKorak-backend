namespace KorakPoKorak.Domain
{
    public static class WorkshopCompletionRules
    {
        /// <summary>
        /// True only for ACTIVE enrollments when every required unit is Done.
        /// Empty workshops (0 activities) are never auto-completed.
        /// </summary>
        public static bool ShouldMarkEnrollmentCompleted(
            EnrollmentStatus enrollmentStatus,
            IReadOnlyCollection<(ActivityUnitType UnitType, int UnitId)> requiredUnits,
            IReadOnlyCollection<(ActivityUnitType UnitType, int UnitId, ActivityProgressStatus Status)> progressRows)
        {
            if (enrollmentStatus != EnrollmentStatus.Active)
                return false;

            if (requiredUnits.Count == 0)
                return false;

            var doneUnits = progressRows
                .Where(p => p.Status == ActivityProgressStatus.Done)
                .Select(p => (p.UnitType, p.UnitId))
                .ToHashSet();

            return requiredUnits.All(unit => doneUnits.Contains((unit.UnitType, unit.UnitId)));
        }
    }
}
