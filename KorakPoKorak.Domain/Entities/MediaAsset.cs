using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Entities
{
    /// <summary>Reusable media/content unit (text body or URL/file reference).</summary>
    public class MediaAsset
    {
        public int Id { get; set; }
        public MediaType Type { get; set; }
        public string? Url { get; set; }
        public string? Content { get; set; }
        public string? MimeType { get; set; }
        public string? FileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}
