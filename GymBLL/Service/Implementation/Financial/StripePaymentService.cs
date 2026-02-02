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

using GymDAL.Repo.Abstract;

namespace GymBLL.Service.Implementation.Financial
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AsyncRetryPolicy _retryPolicy;

        public StripePaymentService(IOptions<StripeSettings> stripeSettings, IUnitOfWork unitOfWork)
        {
            _stripeSettings = stripeSettings.Value;
            _unitOfWork = unitOfWork;
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
            var membership = await _unitOfWork.Memberships.GetByIdAsync(membershipId);
            if (membership == null) throw new Exception("Membership not found");

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
                        { "membershipId", membershipId.ToString() },
                        { "actionType", "InitialRegistration" }
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                UnitAmount = (long)(membership.Price * 100),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = membership.MembershipType,
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

        public async Task<string> CreateSubscriptionCheckoutSessionAsync(string email, int membershipId, string memberId, string actionType, string successUrl, string cancelUrl)
        {
            var membership = await _unitOfWork.Memberships.GetByIdAsync(membershipId);
            if (membership == null) throw new Exception("Membership not found");

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
                        { "memberId", memberId },
                        { "membershipId", membershipId.ToString() },
                        { "actionType", actionType } // "Renew" or "Upgrade"
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                UnitAmount = (long)(membership.Price * 100),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"{membership.MembershipType} ({actionType})",
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

    }
}
