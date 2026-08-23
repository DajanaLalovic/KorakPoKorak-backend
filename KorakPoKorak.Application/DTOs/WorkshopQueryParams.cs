using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class WorkshopQueryParams
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
        public WorkshopStatus? Status { get; set; }
        public WorkshopComplexity? Complexity { get; set; }
        public string? Search { get; set; }
        public bool OnlyMine { get; set; } = false;
        public int? CreatedById { get; set; }
        public int? ContributorId { get; set; }
    }
}
