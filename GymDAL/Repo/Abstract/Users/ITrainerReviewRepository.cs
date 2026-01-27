using GymDAL.Entities.Users;

namespace GymDAL.Repo.Abstract.Users
{
    public interface ITrainerReviewRepository : IRepository<TrainerReview>
    {
        // Add specific methods if needed, e.g., GetReviewsByTrainerIdAsync
        Task<TrainerReview > GetDetailed(string memberId, string trainerId);
    }
}
