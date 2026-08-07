namespace KorakPoKorak.Application.DTOs
{
    public class CreateQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        /// <summary>SINGLE_CHOICE | MULTI_CHOICE | TRUE_FALSE</summary>
        public string Type { get; set; } = "SINGLE_CHOICE";
        public int Points { get; set; } = 1;
        public int OrderIndex { get; set; }
        public List<CreateAnswerDto> Answers { get; set; } = new();
    }
}
