using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int ChildProfileId { get; set; }
        public ChildProfile ChildProfile { get; set; } = null!;

        public int WorkshopId { get; set; }
        public Workshop Workshop { get; set; } = null!;

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime StatusChangedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ActivityProgress> ActivityProgresses { get; set; } = new List<ActivityProgress>();
    }
}
