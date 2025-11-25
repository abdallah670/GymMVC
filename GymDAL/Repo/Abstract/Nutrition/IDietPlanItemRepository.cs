using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Nutrition
{
    public interface IDietPlanItemRepository : IRepository<DietPlanItem>
    {
        // GET Operations
        Task<DietPlanItem> GetByAsync(Expression<Func<DietPlanItem, bool>>? Filter);
        Task<IEnumerable<DietPlanItem>> GetAsync(Expression<Func<DietPlanItem, bool>>? Filter);

        // CREATE Operations
        Task<DietPlanItem> CreateAsync(DietPlanItem item, string Createdby);

        // UPDATE Operations
        Task<DietPlanItem> UpdateAsync(DietPlanItem item, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int itemId, string DeletedBy);
    }
}