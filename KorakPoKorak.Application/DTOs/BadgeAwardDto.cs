using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class BadgeAwardDto
    {
        public int Id { get; set; }
        public int BadgeTemplateId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public BadgeCategory Category { get; set; }
        public int ChildProfileId { get; set; }
        public int? WorkshopId { get; set; }
        public string? WorkshopTitle { get; set; }
        public int? EnrollmentId { get; set; }
        public DateTime AwardedAt { get; set; }
    }
}
