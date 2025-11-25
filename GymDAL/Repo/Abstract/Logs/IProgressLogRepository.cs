using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Logs
{
    public interface IProgressLogRepository : IRepository<ProgressLog>
    {
        // GET Operations
        Task<ProgressLog> GetByAsync(Expression<Func<ProgressLog, bool>>? Filter);
        Task<IEnumerable<ProgressLog>> GetAsync(Expression<Func<ProgressLog, bool>>? Filter);

        // CREATE Operations
        Task<ProgressLog> CreateAsync(ProgressLog log, string Createdby);

        // UPDATE Operations
        Task<ProgressLog> UpdateAsync(ProgressLog log, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int logId, string DeletedBy);
    }
}