using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class ContentBlockDto
    {
        public int Id { get; set; }
        public MediaType Type { get; set; }
        public string? Url { get; set; }
        public string? Content { get; set; }
        public string? MimeType { get; set; }
        public string? FileName { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
