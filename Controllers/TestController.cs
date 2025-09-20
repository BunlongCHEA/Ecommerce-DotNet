using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestController> _logger;

        public TestController(IConfiguration configuration, ILogger<TestController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("sendgrid-direct-test")]
        public async Task<IActionResult> DirectSendGridTest([FromBody] TestEmailRequest request)
        {
            try
            {
                var apiKey = _configuration["SendGrid:ApiKey"];
                var fromEmail = _configuration["SendGrid:FromEmail"]; // Should be sendgrid@bunlong.site
                var fromName = _configuration["SendGrid:FromName"];

                _logger.LogInformation($"Using API Key prefix: {apiKey?.Substring(0, 10)}...");
                _logger.LogInformation($"From Email: {fromEmail}");
                _logger.LogInformation($"To Email: {request.Email}");

                var client = new SendGrid.SendGridClient(apiKey);
                var from = new SendGrid.Helpers.Mail.EmailAddress(fromEmail, fromName);
                var to = new SendGrid.Helpers.Mail.EmailAddress(request.Email);

                var plainTextContent = "This is a test email from SendGrid";
                var htmlContent = "<h1>Test Email</h1><p>This is a test email from SendGrid. If you receive this, the configuration is working!</p>";

                var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, to, "SendGrid Test Email", plainTextContent, htmlContent);

                var response = await client.SendEmailAsync(msg);
                var responseBody = await response.Body.ReadAsStringAsync();

                _logger.LogInformation($"SendGrid Response: {response.StatusCode}");
                _logger.LogInformation($"SendGrid Body: {responseBody}");

                return Ok(new
                {
                    statusCode = response.StatusCode.ToString(),
                    success = response.StatusCode == System.Net.HttpStatusCode.Accepted,
                    responseBody = responseBody,
                    fromEmail = fromEmail,
                    toEmail = request.Email,
                    messageId = response.Headers.FirstOrDefault(h => h.Key == "X-Message-Id").Value?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in direct SendGrid test");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }

    public class TestEmailRequest
    {
        public string? Email { get; set; }
    }
}