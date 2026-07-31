using KorakPoKorak.Application.IRepositories;
using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Role> GetAll()
        {
            return _context.Roles.ToList();
        }

        public Role? GetById(int id)
        {
            return _context.Roles.Find(id);
        }

        public Role? GetByRoleType(UserRole roleType)
        {
            return _context.Roles.FirstOrDefault(r => r.RoleName == roleType);
        }
    }
}
