namespace KorakPoKorak.Application.DTOs
{
    public class QuizQueryParams
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
        public string? Search { get; set; }
        public bool OnlyMine { get; set; } = false;
        public int? CreatedById { get; set; }
    }
}
