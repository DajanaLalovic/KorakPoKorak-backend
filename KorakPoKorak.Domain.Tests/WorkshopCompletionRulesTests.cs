using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Tests
{
    public class WorkshopCompletionRulesTests
    {
        private static readonly (ActivityUnitType UnitType, int UnitId) Lesson1 = (ActivityUnitType.Lesson, 1);
        private static readonly (ActivityUnitType UnitType, int UnitId) Exercise1 = (ActivityUnitType.Exercise, 1);

        private static readonly (ActivityUnitType UnitType, int UnitId)[] Required =
            [Lesson1, Exercise1];

        [Fact]
        public void PartialProgress_KeepsEnrollmentActive()
        {
            var progress = new List<(ActivityUnitType, int, ActivityProgressStatus)>
            {
                (ActivityUnitType.Lesson, 1, ActivityProgressStatus.Done),
                (ActivityUnitType.Exercise, 1, ActivityProgressStatus.InProgress)
            };

            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Active,
                Required,
                progress);

            Assert.False(shouldComplete);
        }

        [Fact]
        public void AllActivitiesDone_MarksEnrollmentCompleted()
        {
            var progress = new List<(ActivityUnitType, int, ActivityProgressStatus)>
            {
                (ActivityUnitType.Lesson, 1, ActivityProgressStatus.Done),
                (ActivityUnitType.Exercise, 1, ActivityProgressStatus.Done)
            };

            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Active,
                Required,
                progress);

            Assert.True(shouldComplete);
        }

        [Fact]
        public void AlreadyCompleted_DoesNotRewrite()
        {
            var progress = new List<(ActivityUnitType, int, ActivityProgressStatus)>
            {
                (ActivityUnitType.Lesson, 1, ActivityProgressStatus.Done),
                (ActivityUnitType.Exercise, 1, ActivityProgressStatus.Done)
            };

            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Completed,
                Required,
                progress);

            Assert.False(shouldComplete);
        }

        [Fact]
        public void Withdrawn_DoesNotComplete()
        {
            var progress = new List<(ActivityUnitType, int, ActivityProgressStatus)>
            {
                (ActivityUnitType.Lesson, 1, ActivityProgressStatus.Done),
                (ActivityUnitType.Exercise, 1, ActivityProgressStatus.Done)
            };

            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Withdrawn,
                Required,
                progress);

            Assert.False(shouldComplete);
        }

        [Fact]
        public void ZeroActivities_DoesNotComplete()
        {
            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Active,
                Array.Empty<(ActivityUnitType, int)>(),
                Array.Empty<(ActivityUnitType, int, ActivityProgressStatus)>());

            Assert.False(shouldComplete);
        }

        [Fact]
        public void ProgressForForeignUnits_DoesNotCountTowardCompletion()
        {
            // Only Lesson 1 belongs to the workshop; Exercise 99 does not.
            var required = new[] { Lesson1 };
            var progress = new List<(ActivityUnitType, int, ActivityProgressStatus)>
            {
                (ActivityUnitType.Exercise, 99, ActivityProgressStatus.Done)
            };

            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Active,
                required,
                progress);

            Assert.False(shouldComplete);
        }

        [Fact]
        public void MissingProgressRow_DoesNotCountAsDone()
        {
            var progress = new List<(ActivityUnitType, int, ActivityProgressStatus)>
            {
                (ActivityUnitType.Lesson, 1, ActivityProgressStatus.Done)
                // Exercise 1 has no progress row yet
            };

            var shouldComplete = WorkshopCompletionRules.ShouldMarkEnrollmentCompleted(
                EnrollmentStatus.Active,
                Required,
                progress);

            Assert.False(shouldComplete);
        }
    }
}
