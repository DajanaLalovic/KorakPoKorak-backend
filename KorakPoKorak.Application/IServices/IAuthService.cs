using KorakPoKorak.Application.DTOs;

namespace KorakPoKorak.Application.IServices
{
    public interface IAuthService
    {
        AuthResponseDto Login(LoginDto dto);
        RegisterResponseDto Register(RegisterDto dto);
        void ActivateAccount(string token);
        void ForgotPassword(ForgotPasswordDto dto);
        void ResetPassword(ResetPasswordDto dto);
    }
}
