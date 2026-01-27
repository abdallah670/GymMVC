using GymBLL.Service.Abstract.Financial;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Financial
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _config;

        public StripeService(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
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
