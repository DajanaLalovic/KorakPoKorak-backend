using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class CreateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public int EstimatedTime { get; set; }
        public ContentStatus Status { get; set; } = ContentStatus.Draft;
    }
}
