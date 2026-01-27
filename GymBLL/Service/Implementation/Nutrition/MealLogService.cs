using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract.Nutrition;
using AutoMapper;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Nutrition;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace GymBLL.Service.Implementation.Nutrition
{
    public class MealLogService : IMealLogService
    {
        public IMapper Mapper
        {
            get;
        }
        public IUnitOfWork UnitOfWork
        {
            get;
        }
        public MealLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        public async Task<Response<MealLogVM>> CreateAsync(MealLogVM model)
        {
            try
            {
                // Ensure that we do not have a duplicate log for the same date and assignment


                var log = Mapper.Map<MealLog>(model);
                await UnitOfWork.MealLogs.AddAsync(log);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    model.Id = log.Id;
                    return new Response<MealLogVM>(model, null, false);
                }
                return new Response<MealLogVM>(null, "Failed to create log.", true);
            }
            catch (Exception ex)
            {
                return new Response<MealLogVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<MealLogVM>> GetByIdAsync(int id)
        {
            try
            {
                var log = await UnitOfWork.MealLogs.GetByIdAsync(id);
                if (log != null)
                {
                    var vm = Mapper.Map<MealLogVM>(log);
                    return new Response<MealLogVM>(vm, null, false);
                }
                return new Response<MealLogVM>(null, "Log not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<MealLogVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<MealLogVM>>> GetAllAsync()
        {
            try
            {
                var vms = ( UnitOfWork.MealLogs.Get(
                    filter: l => l.IsActive,
                    orderBy: q => q.OrderByDescending(l => l.Date),
                    include: q => q.Include(l => l.DietPlanAssignment).ThenInclude(dpa => dpa.DietPlan))).Select(l => Mapper.Map<MealLogVM>(l)).ToList();
                return new Response<List<MealLogVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MealLogVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<MealLogVM>>> GetByDietPlanAssignmentIdAsync(int assignmentId)
        {
            try
            {
                var logs = await UnitOfWork.MealLogs.FindAsync(l => l.DietPlanAssignmentId == assignmentId);
                var vms = logs.Select(l => Mapper.Map<MealLogVM>(l)).ToList();
                return new Response<List<MealLogVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MealLogVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<MealLogVM>> UpdateAsync(MealLogVM model)
        {
            try
            {
                var log = await UnitOfWork.MealLogs.GetByIdAsync(model.Id);
                if (log == null) return new Response<MealLogVM>(null, "Log not found.", true);
                log.DietPlanAssignmentId = model.DietPlanAssignmentId;
                log.Date = model.Date;
                log.MealsConsumed = model.MealsConsumed;
                log.Notes = model.Notes;
                log.CaloriesConsumed = model.CaloriesConsumed;
                log.IsActive = model.IsActive;
                UnitOfWork.MealLogs.Update(log);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<MealLogVM>(model, null, false);
                return new Response<MealLogVM>(null, "Failed to update log.", true);
            }
            catch (Exception ex)
            {
                return new Response<MealLogVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var log = await UnitOfWork.MealLogs.GetByIdAsync(id);
                if (log == null) return new Response<bool>(false, "Log not found.", true);
                UnitOfWork.MealLogs.Remove(log);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete log.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> ToggleStatusAsync(int id)
        {
            try
            {
                var log = await UnitOfWork.MealLogs.GetByIdAsync(id);
                if (log == null) return new Response<bool>(false, "Log not found.", true);
                log.ToggleStatus();
                UnitOfWork.MealLogs.Update(log);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to toggle status.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
    }
}

