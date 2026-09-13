using GymBLL.Service.Abstract.Financial;
using Microsoft.Extensions.Options;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Financial
{
    public class StripeService : IStripeService
    {
        public StripeService(IOptions<StripeSettings> options)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        public async Task<string> CreatePaymentIntentAsync(double amount, string currency, string description)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe expects amounts in cents
                Currency = currency.ToLower(),
                Description = description,
                PaymentMethodTypes = new List<string> { "card" },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);
            return intent.ClientSecret;
        }
    }
}
