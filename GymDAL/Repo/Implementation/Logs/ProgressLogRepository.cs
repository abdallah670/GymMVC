using AutoMapper;
using GymDAL.Repo.Abstract.Logs;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class ProgressLogRepository : Repository<ProgressLog>, IProgressLogRepository
    {
        public ProgressLogRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<ProgressLog> CreateAsync(ProgressLog log, string Createdby)
        {
            try
            {
                log.CreatedBy = Createdby;
                log.CreatedAt = DateTime.UtcNow;
                _context.ProgressLogs.Add(log);
                _context.SaveChanges();
                return Task.FromResult(log);
            }
            catch (Exception)
            {
                return Task.FromResult<ProgressLog>(null);
            }
        }

        public Task<IEnumerable<ProgressLog>> GetAsync(Expression<Func<ProgressLog, bool>>? Filter)
        {
            try
            {
                var query = _context.ProgressLogs.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<ProgressLog>>(null);
            }
        }

        public Task<ProgressLog> GetByAsync(Expression<Func<ProgressLog, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.ProgressLogs.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<ProgressLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<ProgressLog>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int logId, string DeletedBy)
        {
            try
            {
                var log = _context.ProgressLogs.FirstOrDefault(l => l.Id == logId);
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

        public Task<ProgressLog> UpdateAsync(ProgressLog log, string UpdatedBy)
        {
            try
            {
                var existingLog = _context.ProgressLogs.FirstOrDefault(l => l.Id == log.Id);
                if (existingLog != null)
                {
                    _mapper.Map(log, existingLog);
                    existingLog.UpdatedBy = UpdatedBy;
                    existingLog.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingLog);
                }
                return Task.FromResult<ProgressLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<ProgressLog>(null);
            }
        }
    }
}