using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Nutrition
{
    public interface IDietPlanRepository : IRepository<DietPlan>
    {
        // GET Operations
        Task<DietPlan> GetByAsync(Expression<Func<DietPlan, bool>>? Filter);
        Task<IEnumerable<DietPlan>> GetAsync(Expression<Func<DietPlan, bool>>? Filter);

        // CREATE Operations
        Task<DietPlan> CreateAsync(DietPlan plan, string Createdby);

        // UPDATE Operations
        Task<DietPlan> UpdateAsync(DietPlan plan, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int planId, string DeletedBy);
    }
}