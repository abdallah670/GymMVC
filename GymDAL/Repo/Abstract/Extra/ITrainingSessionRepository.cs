using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Extra
{
    public interface ITrainingSessionRepository : IRepository<TrainingSession>
    {
        // GET Operations
        Task<TrainingSession> GetByAsync(Expression<Func<TrainingSession, bool>>? Filter);
        Task<IEnumerable<TrainingSession>> GetAsync(Expression<Func<TrainingSession, bool>>? Filter);

        // CREATE Operations
        Task<TrainingSession> CreateAsync(TrainingSession session, string Createdby);

        // UPDATE Operations
        Task<TrainingSession> UpdateAsync(TrainingSession session, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int sessionId, string DeletedBy);
    }
}