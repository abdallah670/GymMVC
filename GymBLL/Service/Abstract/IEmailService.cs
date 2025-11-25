using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string userName);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}