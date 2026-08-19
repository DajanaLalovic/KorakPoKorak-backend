using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    /// <summary>
    /// Input for one content block. Maps from frontend ContentBlock:
    /// type, text → content, url, mimeType, orderIndex.
    /// </summary>
    public class ContentBlockInputDto
    {
        public MediaType Type { get; set; }
        public string? Url { get; set; }
        public string? Content { get; set; }
        public string? MimeType { get; set; }
        public string? FileName { get; set; }
        public int OrderIndex { get; set; }
    }
}
