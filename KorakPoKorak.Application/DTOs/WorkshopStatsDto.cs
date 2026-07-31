namespace KorakPoKorak.Application.DTOs
{
    public class WorkshopStatsDto
    {
        public int WorkshopId { get; set; }
        public int LessonCount { get; set; }
        public int ExerciseCount { get; set; }
        public int EnrollmentCount { get; set; }
        public double CompletionRate { get; set; }
    }
}
