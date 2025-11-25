using GymBLL.ModelVM.Workout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IWorkoutPlanItemService
    {
        Task<Response<WorkoutPlanItemVM>> CreateAsync(WorkoutPlanItemVM model);
        Task<Response<WorkoutPlanItemVM>> GetByIdAsync(int id);
        Task<Response<List<WorkoutPlanItemVM>>> GetAllAsync();
        Task<Response<List<WorkoutPlanItemVM>>> GetByWorkoutPlanIdAsync(int workoutPlanId);
        Task<Response<WorkoutPlanItemVM>> UpdateAsync(WorkoutPlanItemVM model);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
