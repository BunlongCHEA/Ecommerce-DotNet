using System.Net;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;

public interface IEmailSender
{
    Task<(bool Success, string Message)> SendEmailAsync(string email, string subject, string htmlMessage);
}

public class SmtpEmailSender : IEmailSender
{
    // private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        // _configuration = configuration;
        _apiKey = configuration["SendGrid:ApiKey"];
        _fromEmail = configuration["SendGrid:FromEmail"];
        _fromName = configuration["SendGrid:FromName"];
        _logger = logger;
    }

    public async Task<(bool Success, string Message)>  SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            var response = await client.SendEmailAsync(msg);

            // Log the response details
            _logger.LogInformation($"SendGrid Response - StatusCode: {response.StatusCode}, Headers: {string.Join(", ", response.Headers)}");

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Email sent successfully to {email}");
                return (true, "Email sent successfully");
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"SendGrid failed - StatusCode: {response.StatusCode}, Body: {responseBody}");
                return (false, $"Failed to send email. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending email.");
            return (false, "Exception occurred while sending email.");
        }
    }

    // public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    // {
    //     var smtpHost = _configuration["Email:Host"];
    //     var smtpPort = int.Parse(_configuration["Email:Port"]);
    //     var smtpUser = _configuration["Email:Username"];
    //     var smtpPass = _configuration["Email:Password"];
    //     var smtpFrom = _configuration["Email:FromEmail"];

    //     var client = new SmtpClient(smtpHost, smtpPort)
    //     {
    //         Port = smtpPort,
    //         Credentials = new NetworkCredential(smtpUser, smtpPass),
    //         EnableSsl = true
    //     };

    //     var mail = new MailMessage(smtpFrom, toEmail, subject, htmlMessage)
    //     {
    //         IsBodyHtml = true
    //     };

    //     await client.SendMailAsync(mail);
    // }
}