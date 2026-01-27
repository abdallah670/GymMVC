using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Communication;
using GymBLL.Service.Abstract.Communication;
using GymBLL.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System; 
using GymBLL.Service.Abstract.Communication;

namespace GymBLL.Service.Implementation.Communication
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IRazorViewRenderer _viewRenderer; // Added IRazorViewRenderer

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IRazorViewRenderer viewRenderer) // Updated constructor
        {
            _configuration = configuration;
            _logger = logger;
            _viewRenderer = viewRenderer; // Initialized IRazorViewRenderer
        }

        public async Task<bool> SendPasswordResetEmailAsync(string userEmail, string userName, string resetLink) // Updated method signature
        {
            try
            {
                var subject = "Reset Your Password - MenoPro Gym";
                // Render the email body using the Razor view renderer
                var body = await _viewRenderer.RenderViewToStringAsync("Emails/PasswordReset", new PasswordResetModel { ResetLink = resetLink, UserName = userName });

                // Call the generic SendEmailAsync method
                return await SendEmailAsync(userEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to render or send password reset email to {Email}", userEmail);
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPass"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@menopro.com";
                var fromName = _configuration["EmailSettings:FromName"] ?? "MenoPro Gym";
                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    
                    // DEBUG: Log the full email body to verify content
                    _logger.LogInformation("---------------- EMAIL BODY START ----------------");
                    _logger.LogInformation(body);
                    _logger.LogInformation("---------------- EMAIL BODY END ------------------");

                    mailMessage.To.Add(toEmail);
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }
    }
}

