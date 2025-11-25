using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Nutrition
{
    public interface IMealLogRepository : IRepository<MealLog>
    {
        // GET Operations
        Task<MealLog> GetByAsync(Expression<Func<MealLog, bool>>? Filter);
        Task<IEnumerable<MealLog>> GetAsync(Expression<Func<MealLog, bool>>? Filter);

        // CREATE Operations
        Task<MealLog> CreateAsync(MealLog log, string Createdby);

        // UPDATE Operations
        Task<MealLog> UpdateAsync(MealLog log, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int logId, string DeletedBy);
    }
}