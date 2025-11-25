using GymBLL.ModelVM.External;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IPaymentService
    {
        Task<Response<PaymentVM>> CreateAsync(PaymentVM model);
        Task<Response<PaymentVM>> GetByIdAsync(int id);
        Task<Response<List<PaymentVM>>> GetAllAsync();
        Task<Response<List<PaymentVM>>> GetByMemberIdAsync(string memberId);
        Task<Response<List<PaymentVM>>> GetByStatusAsync(string status);
        Task<Response<PaymentVM>> UpdateAsync(PaymentVM model);
        Task<Response<bool>> ProcessPaymentAsync(int id);
        Task<Response<bool>> RefundPaymentAsync(int id);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
