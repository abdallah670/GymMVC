using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Nutrition
{
    public interface IDietPlanAssignmentRepository : IRepository<DietPlanAssignment>
    {
        // GET Operations
        Task<DietPlanAssignment> GetByAsync(Expression<Func<DietPlanAssignment, bool>>? Filter);
        Task<IEnumerable<DietPlanAssignment>> GetAsync(Expression<Func<DietPlanAssignment, bool>>? Filter);

        // CREATE Operations
        Task<DietPlanAssignment> CreateAsync(DietPlanAssignment assignment, string Createdby);

        // UPDATE Operations
        Task<DietPlanAssignment> UpdateAsync(DietPlanAssignment assignment, string UpdatedBy);

        // DELETE Operations
        Task<bool> ToggleStatusAsync(int assignmentId, string DeletedBy);
    }
}