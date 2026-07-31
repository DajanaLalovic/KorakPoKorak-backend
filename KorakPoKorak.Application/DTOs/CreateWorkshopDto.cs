using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class CreateWorkshopDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public WorkshopComplexity ComplexityLevel { get; set; }
        public List<string> ActivityTypes { get; set; } = new();
        public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;
        public List<int> LessonIds { get; set; } = new();
        public List<int> ExerciseIds { get; set; } = new();
    }
}
