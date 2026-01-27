using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM;
using GymBLL.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Workout
{
    public interface IWorkoutPlanService
    {
        Task<Response<WorkoutPlanVM>> CreateWorkoutPlanAsync(WorkoutPlanVM workoutPlanVm);
        Task<Response<WorkoutPlanVM>> GetWorkoutPlanByIdAsync(int workoutPlanId);
        Task<Response<List<WorkoutPlanVM>>> GetAllWorkoutPlansAsync();
        Task<Response<List<WorkoutPlanVM>>> GetActiveWorkoutPlansAsync();
        Task<Response<WorkoutPlanVM>> UpdateWorkoutPlanAsync(WorkoutPlanVM workoutPlanVm);
        Task<Response<bool>> DeleteWorkoutPlanAsync(int workoutPlanId);
    
        Task<Response<string>> GetWorkoutPlanNameAsync(int Id);
        
        // Pagination
        Task<Response<PagedResult<WorkoutPlanVM>>> GetPagedWorkoutPlansAsync(int pageNumber, int pageSize);
    }
}
