using KorakPoKorak.Domain;

namespace KorakPoKorak.Application.DTOs
{
    public class UpdateActivityProgressDto
    {
        public ActivityUnitType UnitType { get; set; }
        public int UnitId { get; set; }
        public ActivityProgressStatus Status { get; set; }
        public ActivityContext? Context { get; set; }
    }
}
