namespace KorakPoKorak.Domain.Entities
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
