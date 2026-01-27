using GymDAL.Repo.Abstract;
using Microsoft.Extensions.Logging;

namespace GymBLL.Service.Implementation
{
    public class CleanupJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CleanupJob> _logger;

        public CleanupJob(IUnitOfWork unitOfWork, ILogger<CleanupJob> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Deletes expired TempRegistration records that have passed their OTP expiry time.
        /// This job is scheduled to run daily via Hangfire.
        /// </summary>
        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting cleanup of expired TempRegistration records...");

            try
            {
                var deletedCount = await _unitOfWork.TempRegistrationRepository.DeleteExpiredAsync();
                
                if (deletedCount > 0)
                {
                    _logger.LogInformation("Successfully deleted {Count} expired TempRegistration records", deletedCount);
                }
                else
                {
                    _logger.LogInformation("No expired TempRegistration records found to delete");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired TempRegistration records");
                throw; // Re-throw to let Hangfire handle retry logic
            }
        }
    }
}
