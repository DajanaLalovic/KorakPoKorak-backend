using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class BadgeRepository : IBadgeRepository
    {
        private readonly AppDbContext _context;

        public BadgeRepository(AppDbContext context)
        {
            _context = context;
        }

        public BadgeTemplate? GetTemplateByCode(string code)
        {
            return _context.BadgeTemplates.FirstOrDefault(t => t.Code == code);
        }

        public bool ExistsForChild(int childId, int templateId)
        {
            return _context.BadgeAwards.Any(a =>
                a.ChildProfileId == childId && a.BadgeTemplateId == templateId);
        }

        public bool ExistsForChildWorkshop(int childId, int templateId, int workshopId)
        {
            return _context.BadgeAwards.Any(a =>
                a.ChildProfileId == childId
                && a.BadgeTemplateId == templateId
                && a.WorkshopId == workshopId);
        }

        public void AddAward(BadgeAward award)
        {
            _context.BadgeAwards.Add(award);
            _context.SaveChanges();
        }

        public List<BadgeAward> GetAwardsForChild(int childId)
        {
            return _context.BadgeAwards
                .Include(a => a.BadgeTemplate)
                .Include(a => a.Workshop)
                .Where(a => a.ChildProfileId == childId)
                .OrderByDescending(a => a.AwardedAt)
                .ToList();
        }
    }
}
