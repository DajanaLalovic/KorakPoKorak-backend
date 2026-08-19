namespace KorakPoKorak.Application.DTOs
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalAdministrators { get; set; }
        public int TotalMentors { get; set; }
        public int TotalChildren { get; set; }
        public int TotalWorkshops { get; set; }
        public int PublishedWorkshops { get; set; }
        public int DraftWorkshops { get; set; }
        public int ArchivedWorkshops { get; set; }
        public int TotalLessons { get; set; }
        public int TotalExercises { get; set; }
    }
}
