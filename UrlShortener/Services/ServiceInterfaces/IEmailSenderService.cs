namespace UrlShortener.Services.ServiceInterfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailASync(string email, string subject, string message);
    }
}
