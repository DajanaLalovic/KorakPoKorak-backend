using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class WorkshopDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public WorkshopComplexity ComplexityLevel { get; set; }
        public List<string> ActivityTypes { get; set; } = new();
        public WorkshopStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public List<int> ContributorIds { get; set; } = new();
        public List<int> LessonIds { get; set; } = new();
        public List<int> ExerciseIds { get; set; } = new();
    }
}
