using AutoMapper;
using GymDAL.Repo.Abstract.Extra;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class TrainingSessionRepository : Repository<TrainingSession>, ITrainingSessionRepository
    {
        public TrainingSessionRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<TrainingSession> CreateAsync(TrainingSession session, string Createdby)
        {
            try
            {
                session.CreatedBy = Createdby;
                session.CreatedAt = DateTime.UtcNow;
                _context.TrainingSessions.Add(session);
                _context.SaveChanges();
                return Task.FromResult(session);
            }
            catch (Exception)
            {
                return Task.FromResult<TrainingSession>(null);
            }
        }

        public Task<IEnumerable<TrainingSession>> GetAsync(Expression<Func<TrainingSession, bool>>? Filter)
        {
            try
            {
                var query = _context.TrainingSessions.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<TrainingSession>>(null);
            }
        }

        public Task<TrainingSession> GetByAsync(Expression<Func<TrainingSession, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.TrainingSessions.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<TrainingSession>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<TrainingSession>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int sessionId, string DeletedBy)
        {
            try
            {
                var session = _context.TrainingSessions.FirstOrDefault(s => s.Id == sessionId);
                if (session != null)
                {
                    session.ToggleStatus(DeletedBy);
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

        public Task<TrainingSession> UpdateAsync(TrainingSession session, string UpdatedBy)
        {
            try
            {
                var existingSession = _context.TrainingSessions.FirstOrDefault(s => s.Id == session.Id);
                if (existingSession != null)
                {
                    _mapper.Map(session, existingSession);
                    existingSession.UpdatedBy = UpdatedBy;
                    existingSession.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingSession);
                }
                return Task.FromResult<TrainingSession>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<TrainingSession>(null);
            }
        }
    }
}