using AutoMapper;
using GymDAL.Repo.Abstract.Workout;

namespace GymDAL.Repo.Implementation
{
    public class WorkoutPlanRepository : Repository<WorkoutPlan>, IWorkoutPlanRepository
    {
        public WorkoutPlanRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<WorkoutPlan> CreateAsync(WorkoutPlan Plan, string Createdby)
        {
            try
            {
                Plan.CreatedBy = Createdby;
                Plan.CreatedAt = DateTime.UtcNow;
                _context.WorkoutPlans.Add(Plan);
                _context.SaveChanges();
                return Task.FromResult(Plan);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutPlan>(null);
            }
        }

        public Task<IEnumerable<WorkoutPlan>> GetAsync(Expression<Func<WorkoutPlan, bool>>? Filter)
        {
            try
            {
                var query = _context.WorkoutPlans.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<WorkoutPlan>>(null);
            }
        }

        public Task<WorkoutPlan> GetByAsync(Expression<Func<WorkoutPlan, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.WorkoutPlans.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<WorkoutPlan>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutPlan>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int planId, string DeletedBy)
        {
            try
            {
                var plan = _context.WorkoutPlans.FirstOrDefault(p => p.Id == planId);
                if (plan != null)
                {
                    plan.ToggleStatus(DeletedBy);
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

        public Task<WorkoutPlan> UpdateAsync(WorkoutPlan plan, string UpdatedBy)
        {
            try
            {
                var existingPlan = _context.WorkoutPlans.FirstOrDefault(p => p.Id == plan.Id);
                if (existingPlan != null)
                {
                    // AutoMapper updates all properties in one line
                    _mapper.Map(plan, existingPlan);

                    // Only update audit fields manually
                    existingPlan.UpdatedBy = UpdatedBy;
                    existingPlan.UpdatedAt = DateTime.UtcNow;

                    _context.SaveChanges();
                    return Task.FromResult(existingPlan);
                }
                return Task.FromResult<WorkoutPlan>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<WorkoutPlan>(null);
            }
        }
    }
}