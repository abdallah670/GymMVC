using GymBLL.Common;
using GymBLL.ModelVM.Trainer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Trainer
{
    public interface ITrainerReviewService
    {
        Task<Response<bool>> CreateReviewAsync(TrainerReviewVM model, string memberId);
        Task<Response<List<TrainerReviewVM>>> GetReviewsForTrainerAsync(string trainerId);
        Task<Response<TrainerReviewVM>> GetReviewByMemberAsync(string memberId, string trainerId);
        Task<Response<double>> GetAverageRatingAsync(string trainerId);
        Task<Response<List<TrainerReviewVM>>> GetTop3ReviewsForTrainerAsync(string trainerId);
        Task<Response<List<TrainerReviewVM>>> GetTop3ReviewsAsync();
    }
}
