using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Entities
{
    public class ChildProfile
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public ChildGender? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ParentId { get; set; }
        public User Parent { get; set; } = null!;
    }
}
