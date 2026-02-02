using GymDAL.Repo.Abstract;
using GymDAL.Enums;
using Microsoft.Extensions.Logging;
using GymBLL.Service.Abstract.Financial;

namespace GymBLL.Service.Implementation.Financial
{
    public class SubscriptionExpiryJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubscriptionExpiryJob> _logger;

        public SubscriptionExpiryJob(IUnitOfWork unitOfWork, ILogger<SubscriptionExpiryJob> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Checks for active subscriptions that have passed their EndDate and marks them as Expired.
        /// This job is scheduled to run daily via Hangfire.
        /// </summary>
        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting subscription expiry check...");

            try
            {
                var now = DateTime.UtcNow;
                var expiredSubscriptions = await _unitOfWork.Subscriptions.FindAsync(s => 
                    s.Status == SubscriptionStatus.Active && s.EndDate < now);

                int count = 0;
                foreach (var sub in expiredSubscriptions)
                {
                    sub.Status = SubscriptionStatus.Expired;
                     _unitOfWork.Subscriptions.Update(sub);
                    count++;
                }

                if (count > 0)
                {
                    await _unitOfWork.SaveAsync();
                    _logger.LogInformation("Successfully marked {Count} subscriptions as Expired", count);
                }
                else
                {
                    _logger.LogInformation("No expired subscriptions found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking subscription expiries");
                throw;
            }
        }
    }
}
