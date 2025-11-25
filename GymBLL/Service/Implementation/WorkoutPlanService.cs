using AutoMapper;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Workout;
using GymDAL.Repo.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation
{
    public class WorkoutPlanService : IWorkoutPlanService
    {
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public WorkoutPlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Response<WorkoutPlanVM>> CreateWorkoutPlanAsync(WorkoutPlanVM workoutPlanVm)
        {
            try
            {
                var workoutPlan = Mapper.Map<WorkoutPlan>(workoutPlanVm);
                await UnitOfWork.WorkoutPlans.AddAsync(workoutPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    workoutPlanVm.Id = workoutPlan.Id;
                    return new Response<WorkoutPlanVM>(workoutPlanVm, null, false);
                }
                else
                {
                    return new Response<WorkoutPlanVM>(null, "Failed to create workout plan.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<WorkoutPlanVM>(null, $"Failed to create workout plan: {ex.Message}", true);
            }
        }

        public async Task<Response<WorkoutPlanVM>> GetWorkoutPlanByIdAsync(int workoutPlanId)
        {
            try
            {
                var workoutPlan = await UnitOfWork.WorkoutPlans.GetByIdAsync(workoutPlanId);
                if (workoutPlan != null)
                {
                    var workoutPlanVm = Mapper.Map<WorkoutPlanVM>(workoutPlan);
                    return new Response<WorkoutPlanVM>(workoutPlanVm, null, false);
                }
                else
                {
                    return new Response<WorkoutPlanVM>(null, "Workout plan not found.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<WorkoutPlanVM>(null, $"Failed to get workout plan: {ex.Message}", true);
            }
        }

        public async Task<Response<List<WorkoutPlanVM>>> GetAllWorkoutPlansAsync()
        {
            try
            {
                var workoutPlans = await UnitOfWork.WorkoutPlans.GetAllAsync();
                var workoutPlanVms = workoutPlans.Select(wp => Mapper.Map<WorkoutPlanVM>(wp)).ToList();
                return new Response<List<WorkoutPlanVM>>(workoutPlanVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutPlanVM>>(null, $"Failed to get workout plans: {ex.Message}", true);
            }
        }

        public async Task<Response<List<WorkoutPlanVM>>> GetActiveWorkoutPlansAsync()
        {
            try
            {
                var workoutPlans = await UnitOfWork.WorkoutPlans.FindAsync(wp => wp.IsActive);
                var workoutPlanVms = workoutPlans.Select(wp => Mapper.Map<WorkoutPlanVM>(wp)).ToList();
                return new Response<List<WorkoutPlanVM>>(workoutPlanVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutPlanVM>>(null, $"Failed to get active workout plans: {ex.Message}", true);
            }
        }

        public async Task<Response<WorkoutPlanVM>> UpdateWorkoutPlanAsync(WorkoutPlanVM workoutPlanVm)
        {
            try
            {
                var existingWorkoutPlan = await UnitOfWork.WorkoutPlans.GetByIdAsync(workoutPlanVm.Id);

                if (existingWorkoutPlan == null)
                {
                    return new Response<WorkoutPlanVM>(null, "Workout plan not found.", true);
                }

                existingWorkoutPlan.Name = workoutPlanVm.Name;
                existingWorkoutPlan.Description = workoutPlanVm.Description;
                existingWorkoutPlan.Difficulty = workoutPlanVm.Difficulty;
                existingWorkoutPlan.Goal = workoutPlanVm.Goal;
                existingWorkoutPlan.IsActive = workoutPlanVm.IsActive;

                UnitOfWork.WorkoutPlans.Update(existingWorkoutPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<WorkoutPlanVM>(workoutPlanVm, null, false);
                }
                else
                {
                    return new Response<WorkoutPlanVM>(null, "Failed to update workout plan.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<WorkoutPlanVM>(null, $"Failed to update workout plan: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteWorkoutPlanAsync(int workoutPlanId)
        {
            try
            {
                var workoutPlan = await UnitOfWork.WorkoutPlans.GetByIdAsync(workoutPlanId);
                if (workoutPlan == null)
                {
                    return new Response<bool>(false, "Workout plan not found.", true);
                }

                UnitOfWork.WorkoutPlans.Remove(workoutPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<bool>(true, null, false);
                }
                else
                {
                    return new Response<bool>(false, "Failed to delete workout plan.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Failed to delete workout plan: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> ToggleStatusAsync(int workoutPlanId)
        {
            try
            {
                var workoutPlan = await UnitOfWork.WorkoutPlans.GetByIdAsync(workoutPlanId);
                if (workoutPlan == null)
                {
                    return new Response<bool>(false, "Workout plan not found.", true);
                }

                workoutPlan.ToggleStatus();
                UnitOfWork.WorkoutPlans.Update(workoutPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<bool>(true, null, false);
                }
                else
                {
                    return new Response<bool>(false, "Failed to toggle workout plan status.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Failed to toggle workout plan status: {ex.Message}", true);
            }
        }
        public async Task<Response<string>> GetWorkoutPlanNameAsync(int Id)
        {
            try
            {

                var workoutPlan = await UnitOfWork.WorkoutPlans.GetWorkoutPlanNameAsync(Id);
                  
                if (!string.IsNullOrEmpty(workoutPlan))
                {
                    return new Response<string>(workoutPlan,null,false);
                }
                return new Response<string>("Workout plan name not found.", null, true);
            }
            catch( Exception ex)
            {
                return new Response<string>($"Failed to get workout plan name: {ex.Message}", null, true);
            }
           
        }

      
    }
}
