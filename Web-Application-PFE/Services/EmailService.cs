using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Web_Application_PFE.Services.Interfaces;
using Microsoft.Extensions.Options;
using Web_Application_PFE.Models;

public class EmailService : IEmailService
{
    private readonly AuthMessageSenderOptions _options;

    public EmailService(IOptions<AuthMessageSenderOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient(_options.SmtpServer, _options.SmtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_options.FromEmail, _options.Password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.FromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}