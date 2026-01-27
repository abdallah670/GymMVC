using GymBLL.ModelVM.Member;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymBLL.ModelVM.Financial;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GymBLL.Service.Abstract.Member
{
    public interface IFitnessGoalsService
    {
        Task<Response<FitnessGoalsVM>> CreateFitnessGoalAsync(FitnessGoalsVM fitnessGoalsVm);
        Task<Response<List<FitnessGoalsVM>>> GetAllFitnessGoalsAsync();
        Task<Response<List<FitnessGoalsVM>>> GetActiveFitnessGoalsAsync();
        Task<Response<FitnessGoalsVM>> GetByIdAsync(int id);
        Task<Response<FitnessGoalsVM>> UpdateFitnessGoalAsync(FitnessGoalsVM fitnessGoalsVm);
        Task<Response<bool>> DeleteFitnessGoalAsync(int id);
    }
}
