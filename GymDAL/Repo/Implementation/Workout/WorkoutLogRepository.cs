using AutoMapper;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation
{
    public class WorkoutLogRepository : Repository<WorkoutLog>, IWorkoutLogRepository
    {
        public WorkoutLogRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<WorkoutLog> CreateAsync(WorkoutLog log, string Createdby)
        {
            try
            {
                log.CreatedBy = Createdby;
                log.CreatedAt = DateTime.UtcNow;
                _context.WorkoutLogs.Add(log);
                _context.SaveChanges();
                return Task.FromResult(log);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutLog>(null);
            }
        }

        public Task<IEnumerable<WorkoutLog>> GetAsync(Expression<Func<WorkoutLog, bool>>? Filter)
        {
            try
            {
                var query = _context.WorkoutLogs.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<WorkoutLog>>(null);
            }
        }

        public Task<WorkoutLog> GetByAsync(Expression<Func<WorkoutLog, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.WorkoutLogs.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<WorkoutLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutLog>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int logId, string DeletedBy)
        {
            try
            {
                var log = _context.WorkoutLogs.FirstOrDefault(l => l.Id == logId);
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

        public Task<WorkoutLog> UpdateAsync(WorkoutLog log, string UpdatedBy)
        {
            try
            {
                var existingLog = _context.WorkoutLogs.FirstOrDefault(l => l.Id == log.Id);
                if (existingLog != null)
                {
                    _mapper.Map(log, existingLog);
                    existingLog.UpdatedBy = UpdatedBy;
                    existingLog.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingLog);
                }
                return Task.FromResult<WorkoutLog>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutLog>(null);
            }
        }
    }
}