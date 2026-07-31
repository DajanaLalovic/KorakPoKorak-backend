namespace KorakPoKorak.Application.DTOs
{
    public class ExerciseQueryParams
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
        public bool? Printable { get; set; }
        public string? Search { get; set; }
    }
}
