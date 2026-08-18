namespace KorakPoKorak.Application.DTOs
{
    public class ChildSummaryDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
