using KorakPoKorak.Domain.Entities;

namespace KorakPoKorak.Application.IServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
