using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class ActivityProgressDto
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int ChildProfileId { get; set; }
        public int WorkshopId { get; set; }
        public ActivityUnitType UnitType { get; set; }
        public int UnitId { get; set; }
        public ActivityProgressStatus Status { get; set; }
        public ActivityContext? Context { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
