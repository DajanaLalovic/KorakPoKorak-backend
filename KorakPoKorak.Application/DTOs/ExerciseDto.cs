using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class ExerciseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EstimatedTime { get; set; }
        public bool IsPrintable { get; set; }
        public ContentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int WorkshopCount { get; set; }
        public List<ContentBlockDto> ContentBlocks { get; set; } = new();
    }
}
