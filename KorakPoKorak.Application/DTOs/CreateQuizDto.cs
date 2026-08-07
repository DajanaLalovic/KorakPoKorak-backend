namespace KorakPoKorak.Application.DTOs
{
    public class CreateQuizDto
    {
        public string Title { get; set; } = string.Empty;
        public List<CreateQuestionDto> Questions { get; set; } = new();
    }
}
