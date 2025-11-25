using GymBLL.ModelVM.Workout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IWorkoutPlanService
    {
        Task<Response<WorkoutPlanVM>> CreateWorkoutPlanAsync(WorkoutPlanVM workoutPlanVm);
        Task<Response<WorkoutPlanVM>> GetWorkoutPlanByIdAsync(int workoutPlanId);
        Task<Response<List<WorkoutPlanVM>>> GetAllWorkoutPlansAsync();
        Task<Response<List<WorkoutPlanVM>>> GetActiveWorkoutPlansAsync();
        Task<Response<WorkoutPlanVM>> UpdateWorkoutPlanAsync(WorkoutPlanVM workoutPlanVm);
        Task<Response<bool>> DeleteWorkoutPlanAsync(int workoutPlanId);
        Task<Response<bool>> ToggleStatusAsync(int workoutPlanId);
        Task<Response<string>> GetWorkoutPlanNameAsync(int Id);
    }
}
