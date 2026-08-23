using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class MentorStudentWorkshopDto
    {
        public int EnrollmentId { get; set; }
        public int WorkshopId { get; set; }
        public string Title { get; set; } = string.Empty;
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    public class MentorStudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public ChildGender? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public List<MentorStudentWorkshopDto> Workshops { get; set; } = new();
    }
}
