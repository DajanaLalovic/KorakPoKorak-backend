namespace KorakPoKorak.Application.IServices
{
    public interface IEmailService
    {
        void SendActivationEmail(string toEmail, string firstName, string activationToken, string? language = null);
        void SendPasswordResetEmail(string toEmail, string firstName, string resetToken, string? language = null);
    }
}
