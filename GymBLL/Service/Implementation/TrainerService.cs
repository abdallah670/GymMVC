using AutoMapper;
using GymBLL.ModelVM.User.AppUser;
using GymBLL.ModelVM.User.Trainer;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Users;
using GymDAL.Repo.Abstract;
using GymDAL.Repo.Implementation;

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation
{
    public class Trainerservice : ITrainerService
    {
      
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        

        public Trainerservice(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
          
            _userManager = userManager;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
           
        }

         

        

      
       

        public async Task<IdentityResult> Register(RegisterUserVM User)
        {
            try
            {
                var trainer = Mapper.Map<Trainer>(User);
                trainer.UserName = User.Email;
                trainer.EmailConfirmed = true;
                trainer.ExperienceYears = 0;
                trainer.Bio = "";
                var result = await _userManager.CreateAsync(trainer, User.Password);
                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(trainer, "Trainer");
                    if (!roleResult.Succeeded)
                    {
                        return IdentityResult.Failed(roleResult.Errors.ToArray());
                    }
                }


                await UnitOfWork.CommitTransactionAsync();
                return result;

            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<Response<TrainerVM>> GetTrainerByIdAsync(string trainerId)
        {
            try
            {
                var trainer = await UnitOfWork.Trainers.FirstOrDefaultAsync(t => t.Id == trainerId);
                if (trainer != null)
                {
                    var trainerVm = Mapper.Map<TrainerVM>(trainer);
                    return new Response<TrainerVM>(trainerVm, null, false);
                }
                else
                {
                    return new Response<TrainerVM>(null, "Failed to Get trainer.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<TrainerVM>(null, "Failed to Get trainer.", true);
            }
        }

      

        public async Task<Response<TrainerVM>> UpdateTrainerAsync(TrainerVM trainerVm)
        {
            try
            {
                var existingTrainer = await UnitOfWork.Trainers.GetByIdAsync(trainerVm.Id);

                if (existingTrainer == null)
                {
                    return new Response<TrainerVM>(null, "Trainer not found.", true);
                }
                existingTrainer.Bio= trainerVm.Bio;
                existingTrainer.ExperienceYears= trainerVm.ExperienceYears;
                existingTrainer.FullName= trainerVm.FullName;
                existingTrainer.Phone= trainerVm.Phone;
                existingTrainer.ProfilePicture= trainerVm.ProfilePicture;
                UnitOfWork.Trainers.Update(existingTrainer);
                
                // Fixed: Properly await the save operation
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<TrainerVM>(trainerVm, null, false);
                }
                else
                {
                    return new Response<TrainerVM>(null, "Failed to update trainer.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<TrainerVM>(null, $"Failed to update trainer: {ex.Message}", true);
            }
        }

        public async Task<Response<TrainerVM>> GetTrainerByEmailAsync(string Email)
        {
            try
            {
                var trainer = await UnitOfWork.Trainers.FirstOrDefaultAsync(t => t.Email == Email);
                if (trainer != null)
                {
                    var trainerVm = Mapper.Map<TrainerVM>(trainer);
                    return new Response<TrainerVM>(trainerVm, null, false);
                }
                else
                {
                    return new Response<TrainerVM>(null, "Failed to Get trainer.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<TrainerVM>(null, "Failed to Get trainer.", true);
            }
        }
    }
}