using GymBLL.ModelVM.Nutrition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IDietPlanItemService
    {
        Task<Response<DietPlanItemVM>> CreateAsync(DietPlanItemVM model);
        Task<Response<DietPlanItemVM>> GetByIdAsync(int id);
        Task<Response<List<DietPlanItemVM>>> GetAllAsync();
        Task<Response<List<DietPlanItemVM>>> GetByDietPlanIdAsync(int dietPlanId);
        Task<Response<DietPlanItemVM>> UpdateAsync(DietPlanItemVM model);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
