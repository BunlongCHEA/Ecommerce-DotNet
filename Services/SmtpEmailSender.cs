using System.Net;
using System.Net.Mail;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
}

public class SmtpEmailSender: IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var smtpHost = _configuration["Email:Host"];
        var smtpPort = int.Parse(_configuration["Email:Port"]);
        var smtpUser = _configuration["Email:Username"];
        var smtpPass = _configuration["Email:Password"];
        var smtpFrom = _configuration["Email:FromEmail"];

        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Port = smtpPort,
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage(smtpFrom, toEmail, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mail);
    }
}