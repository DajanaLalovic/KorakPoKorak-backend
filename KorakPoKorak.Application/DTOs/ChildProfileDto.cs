using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class ChildProfileDto
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
    }
}
