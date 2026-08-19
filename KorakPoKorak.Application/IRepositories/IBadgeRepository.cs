using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IBadgeRepository
    {
        BadgeTemplate? GetTemplateByCode(string code);
        bool ExistsForChild(int childId, int templateId);
        bool ExistsForChildWorkshop(int childId, int templateId, int workshopId);
        void AddAward(BadgeAward award);
        List<BadgeAward> GetAwardsForChild(int childId);
    }
}
