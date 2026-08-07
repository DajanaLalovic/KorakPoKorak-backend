using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IAuthService
    {
        AuthResponseDto Login(LoginDto dto);
        AuthResponseDto Register(RegisterDto dto);
    }
}
