namespace KorakPoKorak.Application.DTOs
{
    public class CreateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public int EstimatedTime { get; set; }
        /// <summary>Optional. Omitted/null = no content blocks.</summary>
        public List<ContentBlockInputDto>? ContentBlocks { get; set; }
    }
}
