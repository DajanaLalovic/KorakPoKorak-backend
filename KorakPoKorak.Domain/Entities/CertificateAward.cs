namespace KorakPoKorak.Domain.Entities
{
    public class CertificateAward
    {
        public int Id { get; set; }

        public int CertificateTemplateId { get; set; }
        public CertificateTemplate CertificateTemplate { get; set; } = null!;

        public int ChildProfileId { get; set; }
        public ChildProfile ChildProfile { get; set; } = null!;

        public int? WorkshopId { get; set; }
        public Workshop? Workshop { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public string ChildFullName { get; set; } = string.Empty;
        public string WorkshopTitle { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
        public string CertificateTitle { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
