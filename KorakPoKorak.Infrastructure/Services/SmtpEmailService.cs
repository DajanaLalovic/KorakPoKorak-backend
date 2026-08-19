using System.Net;
using System.Net.Mail;
using KorakPoKorak.Application.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KorakPoKorak.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void SendActivationEmail(string toEmail, string firstName, string activationToken)
        {
            var frontendBase = _config["Email:FrontendBaseUrl"] ?? "http://localhost:4200";
            var link = $"{frontendBase.TrimEnd('/')}/activate?token={Uri.EscapeDataString(activationToken)}";

            var subject = "Activate your KorakPoKorak account";
            var body = $@"
                <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:560px;margin:0 auto;color:#1e293b"">
                  <h2 style=""color:#4f46e5"">Confirm your account</h2>
                  <p>Hi {WebUtility.HtmlEncode(firstName)},</p>
                  <p>Thank you for registering on KorakPoKorak. Please click the button below to activate your account:</p>
                  <p style=""margin:28px 0"">
                    <a href=""{link}"" style=""background:#4f46e5;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;font-weight:700"">
                      Activate account
                    </a>
                  </p>
                  <p>Or paste this link into your browser:</p>
                  <p style=""word-break:break-all;color:#64748b;font-size:13px"">{link}</p>
                  <p style=""color:#94a3b8;font-size:12px"">This link expires in 24 hours.</p>
                </div>";

            var host = _config["Email:SmtpHost"];
            var user = _config["Email:User"];
            var password = _config["Email:Password"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "SMTP is not configured. Activation email for {Email} was not sent. Link: {Link}",
                    toEmail, link);
                return;
            }

            var port = int.TryParse(_config["Email:SmtpPort"], out var p) ? p : 587;
            var enableSsl = !bool.TryParse(_config["Email:EnableSsl"], out var ssl) || ssl;
            var fromEmail = _config["Email:From"];
            if (string.IsNullOrWhiteSpace(fromEmail))
                fromEmail = user;

            var fromName = _config["Email:FromName"];
            if (string.IsNullOrWhiteSpace(fromName))
                fromName = "KorakPoKorak";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            try
            {
                client.Send(message);
                _logger.LogInformation("Activation email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send activation email to {Email}. Link: {Link}", toEmail, link);
            }
        }
    }
}
