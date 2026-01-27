using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymBLL.ModelVM.Workout;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GymBLL.Service.Abstract.Workout
{
    public interface IWorkoutAssignmentService
    {
        Task<Response<WorkoutAssignmentVM>> CreateAsync(WorkoutAssignmentVM model);
        Task<Response<WorkoutAssignmentVM>> GetByIdAsync(int id);
        Task<Response<List<WorkoutAssignmentVM>>> GetAllAsync();
        Task<Response<List<WorkoutAssignmentVM>>> GetByWorkoutPlanIdAsync(int workoutPlanId);
        Task<Response<WorkoutAssignmentVM>> UpdateAsync(WorkoutAssignmentVM model);

    }
}
