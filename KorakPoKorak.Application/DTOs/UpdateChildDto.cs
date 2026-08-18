using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class UpdateChildDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public ChildGender? Gender { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
