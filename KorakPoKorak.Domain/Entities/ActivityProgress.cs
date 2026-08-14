using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Entities
{
    public class ActivityProgress
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public ActivityUnitType UnitType { get; set; }
        public int UnitId { get; set; }

        public ActivityProgressStatus Status { get; set; } = ActivityProgressStatus.NotStarted;
        public ActivityContext? Context { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
