using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class ChildRepository : IChildRepository
    {
        private readonly AppDbContext _context;

        public ChildRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<ChildProfile> GetByParent(int parentId)
        {
            return _context.ChildProfiles
                .Where(c => c.ParentId == parentId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public ChildProfile? GetByIdForParent(int childId, int parentId)
        {
            return _context.ChildProfiles
                .FirstOrDefault(c => c.Id == childId && c.ParentId == parentId);
        }

        public void Add(ChildProfile child)
        {
            _context.ChildProfiles.Add(child);
            _context.SaveChanges();
        }

        public void Update(ChildProfile child)
        {
            _context.ChildProfiles.Update(child);
            _context.SaveChanges();
        }

        public void Delete(ChildProfile child)
        {
            _context.ChildProfiles.Remove(child);
            _context.SaveChanges();
        }
    }
}
