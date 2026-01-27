using GymBLL.ModelVM.Trainer;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GymBLL.Service.Abstract.Trainer
{
    public interface ITrainerService
    {
        Task<Response<TrainerVM>> GetTrainerByIdAsync(string trainerId);
        Task<Response<TrainerVM>> GetTrainerByEmailAsync(string Email);
        Task<Response<TrainerVM>> UpdateTrainerAsync(TrainerVM trainerVm);
        Task<IdentityResult> Register(RegisterUserVM User);
    }
}
