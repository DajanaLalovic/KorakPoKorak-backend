namespace KorakPoKorak.Application.DTOs
{
    public class CertificateAwardDto
    {
        public int Id { get; set; }
        public int CertificateTemplateId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ChildProfileId { get; set; }
        public string ChildFullName { get; set; } = string.Empty;
        public int? WorkshopId { get; set; }
        public string WorkshopTitle { get; set; } = string.Empty;
        public int EnrollmentId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
    }
}
