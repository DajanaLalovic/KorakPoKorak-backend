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

        public void SendActivationEmail(string toEmail, string firstName, string activationToken, string? language = null)
        {
            var frontendBase = _config["Email:FrontendBaseUrl"] ?? "http://localhost:4200";
            var link = $"{frontendBase.TrimEnd('/')}/activate?token={Uri.EscapeDataString(activationToken)}";
            var safeName = WebUtility.HtmlEncode(firstName);
            var isSerbian = IsSerbian(language);

            var subject = isSerbian
                ? "Aktivirajte nalog na KorakPoKorak"
                : "Activate your KorakPoKorak account";

            var body = isSerbian
                ? $@"
                <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:560px;margin:0 auto;color:#1e293b"">
                  <h2 style=""color:#4f46e5"">Potvrdite nalog</h2>
                  <p>Zdravo {safeName},</p>
                  <p>Hvala što ste se registrovali na KorakPoKorak. Kliknite na dugme ispod da aktivirate nalog:</p>
                  <p style=""margin:28px 0"">
                    <a href=""{link}"" style=""background:#4f46e5;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;font-weight:700"">
                      Aktiviraj nalog
                    </a>
                  </p>
                  <p>Ili nalepite ovaj link u pregledač:</p>
                  <p style=""word-break:break-all;color:#64748b;font-size:13px"">{link}</p>
                  <p style=""color:#94a3b8;font-size:12px"">Ovaj link ističe za 24 sata.</p>
                </div>"
                : $@"
                <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:560px;margin:0 auto;color:#1e293b"">
                  <h2 style=""color:#4f46e5"">Confirm your account</h2>
                  <p>Hi {safeName},</p>
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

            SendHtml(toEmail, subject, body, "activation", link);
        }

        public void SendPasswordResetEmail(string toEmail, string firstName, string resetToken, string? language = null)
        {
            var frontendBase = _config["Email:FrontendBaseUrl"] ?? "http://localhost:4200";
            var link = $"{frontendBase.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(resetToken)}";
            var safeName = WebUtility.HtmlEncode(firstName);
            var isSerbian = IsSerbian(language);

            var subject = isSerbian
                ? "Resetujte lozinku na KorakPoKorak"
                : "Reset your KorakPoKorak password";

            var body = isSerbian
                ? $@"
                <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:560px;margin:0 auto;color:#1e293b"">
                  <h2 style=""color:#4f46e5"">Nova lozinka</h2>
                  <p>Zdravo {safeName},</p>
                  <p>Primili smo zahtev za resetovanje lozinke. Kliknite na dugme ispod da postavite novu lozinku:</p>
                  <p style=""margin:28px 0"">
                    <a href=""{link}"" style=""background:#4f46e5;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;font-weight:700"">
                      Resetuj lozinku
                    </a>
                  </p>
                  <p>Ili nalepite ovaj link u pregledač:</p>
                  <p style=""word-break:break-all;color:#64748b;font-size:13px"">{link}</p>
                  <p style=""color:#94a3b8;font-size:12px"">Ovaj link ističe za 24 sata. Ako niste tražili reset, ignorišite ovaj email.</p>
                </div>"
                : $@"
                <div style=""font-family:Segoe UI,Arial,sans-serif;max-width:560px;margin:0 auto;color:#1e293b"">
                  <h2 style=""color:#4f46e5"">Reset your password</h2>
                  <p>Hi {safeName},</p>
                  <p>We received a request to reset your password. Click the button below to choose a new one:</p>
                  <p style=""margin:28px 0"">
                    <a href=""{link}"" style=""background:#4f46e5;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;font-weight:700"">
                      Reset password
                    </a>
                  </p>
                  <p>Or paste this link into your browser:</p>
                  <p style=""word-break:break-all;color:#64748b;font-size:13px"">{link}</p>
                  <p style=""color:#94a3b8;font-size:12px"">This link expires in 24 hours. If you did not request a reset, you can ignore this email.</p>
                </div>";

            SendHtml(toEmail, subject, body, "password reset", link);
        }

        private static bool IsSerbian(string? language) =>
            string.Equals(language, "srb", StringComparison.OrdinalIgnoreCase)
            || string.Equals(language, "sr", StringComparison.OrdinalIgnoreCase);

        private void SendHtml(string toEmail, string subject, string body, string kind, string link)
        {
            var host = _config["Email:SmtpHost"];
            var user = _config["Email:User"]?.Trim();
            var password = _config["Email:Password"]?.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "SMTP is not configured. {Kind} email for {Email} was not sent. Link: {Link}",
                    kind, toEmail, link);
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
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
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
                _logger.LogInformation("{Kind} email sent to {Email}", kind, toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send {Kind} email to {Email}. Link: {Link}", kind, toEmail, link);
            }
        }
    }
}
