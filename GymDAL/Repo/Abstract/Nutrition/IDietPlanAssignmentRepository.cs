using System.Linq.Expressions;

namespace GymDAL.Repo.Abstract.Nutrition
{
    public interface IDietPlanAssignmentRepository : IRepository<DietPlanAssignment>
    {
        Task<DietPlanAssignment> GetDetailed(int? dietPlanAssignmentId);
    }
}