using AutoMapper;
using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Nutrition;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation
{
    public class DietPlanService : IDietPlanService
    {
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public DietPlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Response<DietPlanVM>> CreateDietPlanAsync(DietPlanVM dietPlanVm)
        {
            try
            {
                var dietPlan = Mapper.Map<DietPlan>(dietPlanVm);
                await UnitOfWork.DietPlans.AddAsync(dietPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    dietPlanVm.Id = dietPlan.Id;
                    return new Response<DietPlanVM>(dietPlanVm, null, false);
                }
                else
                {
                    return new Response<DietPlanVM>(null, "Failed to create diet plan.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<DietPlanVM>(null, $"Failed to create diet plan: {ex.Message}", true);
            }
        }

        public async Task<Response<DietPlanVM>> GetDietPlanByIdAsync(int dietPlanId)
        {
            try
            {
                var dietPlan = await UnitOfWork.DietPlans.GetByIdAsync(dietPlanId);
                if (dietPlan != null)
                {
                    var dietPlanVm = Mapper.Map<DietPlanVM>(dietPlan);
                    return new Response<DietPlanVM>(dietPlanVm, null, false);
                }
                else
                {
                    return new Response<DietPlanVM>(null, "Diet plan not found.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<DietPlanVM>(null, $"Failed to get diet plan: {ex.Message}", true);
            }
        }

        public async Task<Response<List<DietPlanVM>>> GetAllDietPlansAsync()
        {
            try
            {
                var dietPlans = await UnitOfWork.DietPlans.GetAllAsync();
                var dietPlanVms = dietPlans.Select(dp => Mapper.Map<DietPlanVM>(dp)).ToList();
                return new Response<List<DietPlanVM>>(dietPlanVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<DietPlanVM>>(null, $"Failed to get diet plans: {ex.Message}", true);
            }
        }

        public async Task<Response<List<DietPlanVM>>> GetActiveDietPlansAsync()
        {
            try
            {
                var dietPlans = await UnitOfWork.DietPlans.FindAsync(dp => dp.IsActive);
                var dietPlanVms = dietPlans.Select(dp => Mapper.Map<DietPlanVM>(dp)).ToList();
                return new Response<List<DietPlanVM>>(dietPlanVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<DietPlanVM>>(null, $"Failed to get active diet plans: {ex.Message}", true);
            }
        }

        public async Task<Response<DietPlanVM>> UpdateDietPlanAsync(DietPlanVM dietPlanVm)
        {
            try
            {
                var existingDietPlan = await UnitOfWork.DietPlans.GetByIdAsync(dietPlanVm.Id);

                if (existingDietPlan == null)
                {
                    return new Response<DietPlanVM>(null, "Diet plan not found.", true);
                }

                existingDietPlan.Name = dietPlanVm.Name;
                existingDietPlan.Description = dietPlanVm.Description;
                existingDietPlan.TotalCalories = dietPlanVm.TotalCalories;
                existingDietPlan.ProteinMacros = dietPlanVm.ProteinMacros;
                existingDietPlan.CarbMacros = dietPlanVm.CarbMacros;
                existingDietPlan.FatMacros = dietPlanVm.FatMacros;
                existingDietPlan.DurationDays = dietPlanVm.DurationDays;
                existingDietPlan.IsActive = dietPlanVm.IsActive;

                UnitOfWork.DietPlans.Update(existingDietPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<DietPlanVM>(dietPlanVm, null, false);
                }
                else
                {
                    return new Response<DietPlanVM>(null, "Failed to update diet plan.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<DietPlanVM>(null, $"Failed to update diet plan: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteDietPlanAsync(int dietPlanId)
        {
            try
            {
                var dietPlan = await UnitOfWork.DietPlans.GetByIdAsync(dietPlanId);
                if (dietPlan == null)
                {
                    return new Response<bool>(false, "Diet plan not found.", true);
                }

                UnitOfWork.DietPlans.Remove(dietPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<bool>(true, null, false);
                }
                else
                {
                    return new Response<bool>(false, "Failed to delete diet plan.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Failed to delete diet plan: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> ToggleStatusAsync(int dietPlanId)
        {
            try
            {
                var dietPlan = await UnitOfWork.DietPlans.GetByIdAsync(dietPlanId);
                if (dietPlan == null)
                {
                    return new Response<bool>(false, "Diet plan not found.", true);
                }

                dietPlan.ToggleStatus();
                UnitOfWork.DietPlans.Update(dietPlan);

                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<bool>(true, null, false);
                }
                else
                {
                    return new Response<bool>(false, "Failed to toggle diet plan status.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Failed to toggle diet plan status: {ex.Message}", true);
            }
        }
    }
}
