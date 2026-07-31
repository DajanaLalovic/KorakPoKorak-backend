using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IAuthService
    {
        AuthResponseDto Login(LoginDto dto);
        void Register(RegisterDto dto);
    }
}
