namespace KorakPoKorak.Domain.Entities
{
    public class CertificateTemplate
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<CertificateAward> Awards { get; set; } = new List<CertificateAward>();
    }
}
