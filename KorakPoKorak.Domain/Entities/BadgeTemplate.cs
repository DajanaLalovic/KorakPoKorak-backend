using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Entities
{
    public class BadgeTemplate
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public BadgeCategory Category { get; set; }
        public BadgeAwardScope Scope { get; set; }

        public ICollection<BadgeAward> Awards { get; set; } = new List<BadgeAward>();
    }
}
