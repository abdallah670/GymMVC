using AutoMapper;
using GymDAL.Repo.Abstract.Nutrition;
using System.Linq.Expressions;

namespace GymDAL.Repo.Implementation
{
    public class DietPlanRepository : Repository<DietPlan>, IDietPlanRepository
    {
        public DietPlanRepository(GymDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public Task<DietPlan> CreateAsync(DietPlan plan, string Createdby)
        {
            try
            {
                plan.CreatedBy = Createdby;
                plan.CreatedAt = DateTime.UtcNow;
                _context.DietPlans.Add(plan);
                _context.SaveChanges();
                return Task.FromResult(plan);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlan>(null);
            }
        }

        public Task<IEnumerable<DietPlan>> GetAsync(Expression<Func<DietPlan, bool>>? Filter)
        {
            try
            {
                var query = _context.DietPlans.AsQueryable();
                if (Filter != null)
                {
                    query = query.Where(Filter);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception)
            {
                return Task.FromResult<IEnumerable<DietPlan>>(null);
            }
        }

        public Task<DietPlan> GetByAsync(Expression<Func<DietPlan, bool>>? Filter)
        {
            try
            {
                if (Filter != null)
                {
                    var result = _context.DietPlans.FirstOrDefault(Filter);
                    return Task.FromResult(result);
                }
                return Task.FromResult<DietPlan>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlan>(null);
            }
        }

        public Task<bool> ToggleStatusAsync(int planId, string DeletedBy)
        {
            try
            {
                var plan = _context.DietPlans.FirstOrDefault(p => p.Id == planId);
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

        public Task<DietPlan> UpdateAsync(DietPlan plan, string UpdatedBy)
        {
            try
            {
                var existingPlan = _context.DietPlans.FirstOrDefault(p => p.Id == plan.Id);
                if (existingPlan != null)
                {
                    _mapper.Map(plan, existingPlan);
                    existingPlan.UpdatedBy = UpdatedBy;
                    existingPlan.UpdatedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Task.FromResult(existingPlan);
                }
                return Task.FromResult<DietPlan>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<DietPlan>(null);
            }
        }
    }
}