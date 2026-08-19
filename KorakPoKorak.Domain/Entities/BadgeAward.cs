using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Entities
{
    public class BadgeAward
    {
        public int Id { get; set; }

        public int BadgeTemplateId { get; set; }
        public BadgeTemplate BadgeTemplate { get; set; } = null!;

        public int ChildProfileId { get; set; }
        public ChildProfile ChildProfile { get; set; } = null!;

        public int? WorkshopId { get; set; }
        public Workshop? Workshop { get; set; }

        public int? EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public BadgeAwardScope Scope { get; set; }
        public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
    }
}
