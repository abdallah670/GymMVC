using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class MealLogRepository : Repository<MealLog>, IMealLogRepository
    {
        public MealLogRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<MealLog> CreateAsync(MealLog log, string Createdby)
        {
            try
            {
                log.CreatedBy = Createdby;
                log.CreatedAt = DateTime.UtcNow;
                _context.MealLogs.Add(log);
                _context.SaveChanges();
                return Task.FromResult(log);
            }
            catch (Exception)
            {
                return Task.FromResult<MealLog>(null);
            }
        }

        public Task<IEnumerable<MealLog>> GetAsync(Expression<Func<MealLog, bool>>? Filter)
        {
            try
            {
                var query = _context.MealLogs.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<MealLog>>(null);
            }
        }

        public Task<MealLog> GetByAsync(Expression<Func<MealLog, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.MealLogs.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<MealLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<MealLog>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int logId, string DeletedBy)
        {
            try
            {
                var log = _context.MealLogs.FirstOrDefault(l => l.Id == logId);
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

        public Task<MealLog> UpdateAsync(MealLog log, string UpdatedBy)
        {
            try
            {
                var existingLog = _context.MealLogs.FirstOrDefault(l => l.Id == log.Id);
                if (existingLog != null)
                {
                    _mapper.Map(log, existingLog);
                    existingLog.UpdatedBy = UpdatedBy;
                    existingLog.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingLog);
                }
                return Task.FromResult<MealLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<MealLog>(null);
            }
        }
    }
}