using AutoMapper;
using GymDAL.Repo.Abstract.Logs;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class ReportLogRepository : Repository<ReportLog>, IReportLogRepository
    {
        public ReportLogRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<ReportLog> CreateAsync(ReportLog log, string Createdby)
        {
            try
            {
                log.CreatedBy = Createdby;
                log.CreatedAt = DateTime.UtcNow;
                _context.ReportLogs.Add(log);
                _context.SaveChanges();
                return Task.FromResult(log);
            }
            catch (Exception)
            {
                return Task.FromResult<ReportLog>(null);
            }
        }

        public Task<IEnumerable<ReportLog>> GetAsync(Expression<Func<ReportLog, bool>>? Filter)
        {
            try
            {
                var query = _context.ReportLogs.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<ReportLog>>(null);
            }
        }

        public Task<ReportLog> GetByAsync(Expression<Func<ReportLog, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.ReportLogs.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<ReportLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<ReportLog>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int logId, string DeletedBy)
        {
            try
            {
                var log = _context.ReportLogs.FirstOrDefault(l => l.Id == logId);
                if (log != null)
                {
                    log.ToggleStatus(DeletedBy);
                    _context.SaveChanges();
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }

        public Task<ReportLog> UpdateAsync(ReportLog log, string UpdatedBy)
        {
            try
            {
                var existingLog = _context.ReportLogs.FirstOrDefault(l => l.Id == log.Id);
                if (existingLog != null)
                {
                    _mapper.Map(log, existingLog);
                    existingLog.UpdatedBy = UpdatedBy;
                    existingLog.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingLog);
                }
                return Task.FromResult<ReportLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<ReportLog>(null);
            }
        }
    }
}