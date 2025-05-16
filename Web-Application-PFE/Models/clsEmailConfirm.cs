using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Web_Application_PFE.Models
{
    public class clsEmailConfirm : IEmailSender
    {
        private readonly AuthMessageSenderOptions _options;
        private readonly ILogger<clsEmailConfirm> _logger;

        public clsEmailConfirm(
            IOptions<AuthMessageSenderOptions> options,
            ILogger<clsEmailConfirm> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("L'adresse email ne peut pas être vide", nameof(email));

            try
            {
                using var client = new SmtpClient(_options.SmtpServer)
                {
                    Port = _options.SmtpPort,
                    Credentials = new NetworkCredential(_options.FromEmail, _options.Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 20000 // 20 secondes timeout
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_options.FromEmail),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };

                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);

                _logger.LogInformation($"Email envoyé avec succès à {email}");
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"Erreur SMTP lors de l'envoi à {email}: {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur générale lors de l'envoi d'email à {email}");
                throw;
            }
        }
    }
}