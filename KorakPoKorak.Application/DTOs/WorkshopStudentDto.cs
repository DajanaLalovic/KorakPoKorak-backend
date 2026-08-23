using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class WorkshopStudentDto
    {
        public int EnrollmentId { get; set; }
        public int ChildProfileId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ChildGender? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
