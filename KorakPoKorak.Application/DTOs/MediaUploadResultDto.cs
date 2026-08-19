namespace KorakPoKorak.Application.DTOs
{
    public class MediaUploadResultDto
    {
        public string Url { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public string? FileName { get; set; }
    }
}
