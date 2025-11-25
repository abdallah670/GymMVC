using GymBLL.ModelVM.External;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IFitnessGoalsService
    {
        Task<Response<FitnessGoalsVM>> CreateFitnessGoalAsync(FitnessGoalsVM fitnessGoalsVm);
        Task<Response<List<FitnessGoalsVM>>> GetAllFitnessGoalsAsync();
        Task<Response<List<FitnessGoalsVM>>> GetActiveFitnessGoalsAsync();
      
    }
}