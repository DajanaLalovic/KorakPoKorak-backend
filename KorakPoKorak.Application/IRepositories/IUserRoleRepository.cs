using KorakPoKorak.Domain;
using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IRoleRepository
    {
        List<Role> GetAll();
        Role? GetById(int id);
        Role? GetByRoleType(UserRole roleType);
    }
}
