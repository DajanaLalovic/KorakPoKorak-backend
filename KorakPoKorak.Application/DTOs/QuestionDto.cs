namespace KorakPoKorak.Application.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        /// <summary>SINGLE_CHOICE | MULTI_CHOICE | TRUE_FALSE</summary>
        public string Type { get; set; } = "SINGLE_CHOICE";
        public int Points { get; set; }
        public int OrderIndex { get; set; }
        public List<AnswerDto> Answers { get; set; } = new();
    }
}
