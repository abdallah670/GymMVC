using GymBLL.ModelVM.Workout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IWorkoutAssignmentService
    {
        Task<Response<WorkoutAssignmentVM>> CreateAsync(WorkoutAssignmentVM model);
        Task<Response<WorkoutAssignmentVM>> GetByIdAsync(int id);
        Task<Response<List<WorkoutAssignmentVM>>> GetAllAsync();
    
        Task<Response<List<WorkoutAssignmentVM>>> GetByWorkoutPlanIdAsync(int workoutPlanId);
        Task<Response<WorkoutAssignmentVM>> UpdateAsync(WorkoutAssignmentVM model);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
