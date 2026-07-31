namespace KorakPoKorak.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public UserRole RoleName { get; set; }
        public string Description { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
