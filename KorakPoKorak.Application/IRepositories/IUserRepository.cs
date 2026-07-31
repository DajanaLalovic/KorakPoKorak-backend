using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IRepositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(int id);
        User? GetByEmail(string email);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
    }
}
