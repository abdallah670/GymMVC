using GymBLL.ModelVM.Communication;
using GymBLL.ModelVM;
using GymBLL.Response;
using System.Threading.Tasks;
namespace GymBLL.Service.Abstract.Communication { 
    public interface IEmailService { 
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string userName); 
        Task<bool> SendEmailAsync(string toEmail, string subject, string body); } 
}
