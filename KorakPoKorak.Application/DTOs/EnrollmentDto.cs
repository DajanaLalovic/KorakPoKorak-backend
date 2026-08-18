using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int ChildProfileId { get; set; }
        public int WorkshopId { get; set; }
        public string WorkshopTitle { get; set; } = string.Empty;
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DateTime StatusChangedAt { get; set; }
    }
}
