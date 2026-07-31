namespace KorakPoKorak.Domain.Entities
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int EstimatedTime { get; set; }
        public bool IsPrintable { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
    }
}
