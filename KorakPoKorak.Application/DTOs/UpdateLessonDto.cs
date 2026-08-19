namespace KorakPoKorak.Application.DTOs
{
    public class UpdateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public int EstimatedTime { get; set; }
        /// <summary>
        /// Optional. null = leave existing blocks unchanged;
        /// empty list = clear all blocks; otherwise replace all blocks.
        /// </summary>
        public List<ContentBlockInputDto>? ContentBlocks { get; set; }
    }
}
