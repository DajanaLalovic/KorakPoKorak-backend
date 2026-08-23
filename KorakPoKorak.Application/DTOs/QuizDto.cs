namespace KorakPoKorak.Application.DTOs
{
    /// <summary>Full quiz response including questions and answers.</summary>
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public int ExerciseCount { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
