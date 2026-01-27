using GymBLL.Common;
using GymBLL.Service.Abstract.Financial;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation.Financial
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly AsyncRetryPolicy _retryPolicy;

        public StripePaymentService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            // Configure Polly retry policy for transient failures
            _retryPolicy = Policy
                .Handle<StripeException>(ex => ex.StripeError?.Type == "api_connection_error" || ex.HttpStatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Stripe API retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                    });
        }

        public async Task<string> CreateCheckoutSessionAsync(string email, int membershipId, int tempRegistrationId, string successUrl, string cancelUrl)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    CustomerEmail = email,
                    SuccessUrl = successUrl + "?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = cancelUrl,
                    Metadata = new Dictionary<string, string>
                    {
                        { "tempRegistrationId", tempRegistrationId.ToString() },
                        { "membershipId", membershipId.ToString() }
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                UnitAmount = GetMembershipPrice(membershipId), // Price in cents
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = GetMembershipName(membershipId),
                                    Description = "MenoPro Gym Membership"
                                }
                            },
                            Quantity = 1
                        }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            });
        }

        private long GetMembershipPrice(int membershipId)
        {
            // TODO: Fetch from database via MembershipService
            return membershipId switch
            {
                1 => 2900,  // Basic $29
                2 => 5900,  // Pro $59
                3 => 9900,  // Elite $99
                _ => 2900   // Default
            };
        }

        private string GetMembershipName(int membershipId)
        {
            return membershipId switch
            {
                1 => "Basic Membership",
                2 => "Pro Membership",
                3 => "Elite Membership",
                _ => "Standard Membership"
            };
        }
    }
}
