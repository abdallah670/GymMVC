using GymBLL.ModelVM.Nutrition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IDietPlanAssignmentService
    {
        Task<Response<DietPlanAssignmentVM>> CreateAsync(DietPlanAssignmentVM model);
        Task<Response<DietPlanAssignmentVM>> GetByIdAsync(int id);
        Task<Response<List<DietPlanAssignmentVM>>> GetAllAsync();
        //Task<Response<List<DietPlanAssignmentVM>>> GetByMemberIdAsync(string memberId);
        Task<Response<List<DietPlanAssignmentVM>>> GetByDietPlanIdAsync(int dietPlanId);
        Task<Response<DietPlanAssignmentVM>> UpdateAsync(DietPlanAssignmentVM model);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
