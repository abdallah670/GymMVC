using GymBLL.Common;
using GymBLL.ModelVM.Workout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Workout
{
    public interface IWorkoutLogService
    {
        Task<Response<bool>> LogWorkoutAsync(WorkoutLogVM model);
        Task<Response<List<WorkoutLogVM>>> GetMemberHistoryAsync(string memberId);
    }
}
