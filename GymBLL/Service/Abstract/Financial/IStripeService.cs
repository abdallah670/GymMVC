using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Financial
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntentAsync(double amount, string currency, string description);
    }
}
