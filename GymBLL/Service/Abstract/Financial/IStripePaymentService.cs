using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Financial
{
    public interface IStripePaymentService
    {
        /// <summary>
        /// Creates a Stripe Checkout Session for the given email and membership.
        /// Returns the Stripe Checkout URL.
        /// </summary>
        Task<string> CreateCheckoutSessionAsync(string email, int membershipId, int tempRegistrationId, string successUrl, string cancelUrl);
    }
}
