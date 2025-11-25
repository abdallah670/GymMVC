using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Logs
{
    public interface ISystemLogRepository : IRepository<SystemLog>
    {
        // GET Operations
        Task<SystemLog> GetByAsync(Expression<Func<SystemLog, bool>>? Filter);
        Task<IEnumerable<SystemLog>> GetAsync(Expression<Func<SystemLog, bool>>? Filter);

        // CREATE Operations
        Task<SystemLog> CreateAsync(SystemLog log, string Createdby);

        // UPDATE Operations
        Task<SystemLog> UpdateAsync(SystemLog log, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int logId, string DeletedBy);
    }
}