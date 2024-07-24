using System.Net;
using System.Net.Mail;
using UrlShortener.Services.ServiceInterfaces;

namespace UrlShortener.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _config;

        public EmailSenderService(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailASync(string email, string subject, string message)
        {
            var EMAIL_LOGIN_NAME = _config["EMAIL_LOGIN_NAME"];
            var EMAIL_PASSWORD = _config["EMAIL_PASSWORD"];

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(EMAIL_LOGIN_NAME, EMAIL_PASSWORD)
            };

            return client.SendMailAsync(
                new MailMessage(EMAIL_LOGIN_NAME, email, subject, message) { IsBodyHtml = true }
            );
        }
    }
}
