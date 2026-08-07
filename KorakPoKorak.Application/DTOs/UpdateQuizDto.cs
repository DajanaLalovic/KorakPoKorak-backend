namespace KorakPoKorak.Application.DTOs
{
    public class UpdateQuizDto
    {
        public string Title { get; set; } = string.Empty;
        public List<CreateQuestionDto> Questions { get; set; } = new();
    }
}
