using System.Linq.Expressions;
using GymDAL.Entities.Progress_and_Notification;

namespace GymDAL.Repo.Abstract.Logs
{
    public interface IReportLogRepository : IRepository<ReportLog>
    {
        // GET Operations
        Task<ReportLog> GetByAsync(Expression<Func<ReportLog, bool>>? Filter);
        Task<IEnumerable<ReportLog>> GetAsync(Expression<Func<ReportLog, bool>>? Filter);

        // CREATE Operations
        Task<ReportLog> CreateAsync(ReportLog log, string Createdby);

        // UPDATE Operations
        Task<ReportLog> UpdateAsync(ReportLog log, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int logId, string DeletedBy);
    }
}