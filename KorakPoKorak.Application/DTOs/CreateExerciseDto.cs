namespace KorakPoKorak.Application.DTOs
{
    public class CreateExerciseDto
    {
        public string Title { get; set; } = string.Empty;
        public int EstimatedTime { get; set; }
        public bool IsPrintable { get; set; }
    }
}
