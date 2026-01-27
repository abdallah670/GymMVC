using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymBLL.ModelVM.Nutrition;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GymBLL.Service.Abstract.Nutrition { public interface IMealLogService { Task<Response<MealLogVM>> CreateAsync(MealLogVM model); Task<Response<MealLogVM>> GetByIdAsync(int id); Task<Response<List<MealLogVM>>> GetAllAsync(); Task<Response<List<MealLogVM>>> GetByDietPlanAssignmentIdAsync(int assignmentId); Task<Response<MealLogVM>> UpdateAsync(MealLogVM model); Task<Response<bool>> DeleteAsync(int id); Task<Response<bool>> ToggleStatusAsync(int id); } }
