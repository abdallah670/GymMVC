using GymBLL.ModelVM.Nutrition;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymBLL.ModelVM;

namespace GymBLL.Service.Abstract
{
    public interface IDietPlanService
    {
        Task<Response<DietPlanVM>> CreateDietPlanAsync(DietPlanVM dietPlanVm);
        Task<Response<DietPlanVM>> GetDietPlanByIdAsync(int dietPlanId);
        Task<Response<List<DietPlanVM>>> GetAllDietPlansAsync();
        Task<Response<List<DietPlanVM>>> GetActiveDietPlansAsync();
        Task<Response<DietPlanVM>> UpdateDietPlanAsync(DietPlanVM dietPlanVm);
        Task<Response<bool>> DeleteDietPlanAsync(int dietPlanId);
        Task<Response<bool>> ToggleStatusAsync(int dietPlanId);
        // Pagination
        Task<Response<PagedResult<DietPlanVM>>> GetPagedDietPlansAsync(int pageNumber, int pageSize);
    }
}
