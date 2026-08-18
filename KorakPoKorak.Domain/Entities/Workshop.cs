namespace KorakPoKorak.Domain.Entities
{
    public class Workshop
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public WorkshopComplexity ComplexityLevel { get; set; }
        public List<string> ActivityTypes { get; set; } = new();
        public WorkshopStatus Status { get; set; } = WorkshopStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public ICollection<User> Contributors { get; set; } = new List<User>();
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
