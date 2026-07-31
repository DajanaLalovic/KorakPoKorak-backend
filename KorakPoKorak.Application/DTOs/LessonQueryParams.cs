namespace KorakPoKorak.Application.DTOs
{
    public class LessonQueryParams
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
        public string? Search { get; set; }
    }
}
