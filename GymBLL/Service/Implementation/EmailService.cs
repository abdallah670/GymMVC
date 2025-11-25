using GymBLL.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string userName)
        {
            var subject = "Reset Your Password - MenoPro Gym";
            var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; color: white; border-radius: 10px 10px 0 0;'>
                    <h1 style='margin: 0;'>🔐 Password Reset</h1>
                </div>
                
                <div style='padding: 30px; background: #f8f9fa; border-radius: 0 0 10px 10px;'>
                    <p style='font-size: 16px; color: #333;'>Hi {userName},</p>
                    
                    <p style='font-size: 16px; color: #333;'>
                        We received a request to reset your password for your MenoPro Gym account.
                        Click the button below to create a new password:
                    </p>
                    
                    <div style='text-align: center; margin: 40px 0;'>
                        <a href='{resetLink}' 
                           style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                  color: white; 
                                  padding: 15px 40px; 
                                  text-decoration: none; 
                                  border-radius: 8px; 
                                  font-weight: bold;
                                  font-size: 16px;
                                  display: inline-block;
                                  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);'>
                            Reset Password
                        </a>
                    </div>
                    
                    <p style='font-size: 14px; color: #666;'>
                        This link will expire in 24 hours for security reasons.
                        If you didn't request a password reset, you can safely ignore this email.
                    </p>
                    
                    <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd;'>
                        <p style='font-size: 12px; color: #888;'>
                            Need help? Contact our support team at support@menopro.com
                        </p>
                    </div>
                </div>
            </div>";

            return await SendEmailAsync(toEmail, subject, body);
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