using GymBLL.Common;
using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Communication;
using GymBLL.Service.Abstract.Communication;
using GymBLL.Service.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;

namespace GymBLL.Service.Implementation.Communication
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IRazorViewRenderer _viewRenderer;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IRazorViewRenderer viewRenderer)
        {
            _settings = settings.Value;
            _logger = logger;
            _viewRenderer = viewRenderer;
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
                if (!_settings.IsConfigured)
                {
                    _logger.LogWarning("Email is not configured (EmailSettings:SmtpUser/SmtpPass missing). Skipping email to {Email}. Set credentials via user-secrets or environment variables.", toEmail);
                    return false;
                }

                using (var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort))
                {
                    client.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass);
                    client.EnableSsl = true;
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_settings.FromEmail, _settings.FromName),
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

