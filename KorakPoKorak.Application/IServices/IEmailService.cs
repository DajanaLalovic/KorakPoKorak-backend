namespace KorakPoKorak.Application.IServices
{
    public interface IEmailService
    {
        void SendActivationEmail(string toEmail, string firstName, string activationToken);
    }
}
