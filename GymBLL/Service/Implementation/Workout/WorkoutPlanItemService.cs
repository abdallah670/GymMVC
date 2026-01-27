using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Workout;
using GymBLL.Service.Abstract.Workout;
using AutoMapper;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Workout;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GymBLL.Service.Implementation.Workout
{
    public class WorkoutPlanItemService : IWorkoutPlanItemService
    {
        public IMapper Mapper
        {
            get;
        }
        public IUnitOfWork UnitOfWork
        {
            get;
        }
        public WorkoutPlanItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        public async Task<Response<WorkoutPlanItemVM>> CreateAsync(WorkoutPlanItemVM model)
        {
            try
            {
                var item = Mapper.Map<WorkoutPlanItem>(model);
                await UnitOfWork.WorkoutPlanItems.AddAsync(item);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    model.Id = item.Id;
                    return new Response<WorkoutPlanItemVM>(model, null, false);
                }
                return new Response<WorkoutPlanItemVM>(null, "Failed to create item.", true);
            }
            catch (Exception ex)
            {
                return new Response<WorkoutPlanItemVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<WorkoutPlanItemVM>> GetByIdAsync(int id)
        {
            try
            {
                var item = await UnitOfWork.WorkoutPlanItems.GetByIdAsync(id);
                if (item != null)
                {
                    var vm = Mapper.Map<WorkoutPlanItemVM>(item);
                    return new Response<WorkoutPlanItemVM>(vm, null, false);
                }
                return new Response<WorkoutPlanItemVM>(null, "Item not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<WorkoutPlanItemVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<WorkoutPlanItemVM>>> GetAllAsync()
        {
            try
            {
                var items = await UnitOfWork.WorkoutPlanItems.GetAllAsync();
                var vms = items.Select(i => Mapper.Map<WorkoutPlanItemVM>(i)).ToList();
                return new Response<List<WorkoutPlanItemVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutPlanItemVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<WorkoutPlanItemVM>>> GetByWorkoutPlanIdAsync(int workoutPlanId)
        {
            try
            {
                var items = await UnitOfWork.WorkoutPlanItems.FindAsync(i => i.WorkoutPlanId == workoutPlanId);
                var vms = items.Select(i => Mapper.Map<WorkoutPlanItemVM>(i)).ToList();
                return new Response<List<WorkoutPlanItemVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutPlanItemVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<WorkoutPlanItemVM>> UpdateAsync(WorkoutPlanItemVM model)
        {
            try
            {
                var item = await UnitOfWork.WorkoutPlanItems.GetByIdAsync(model.Id);
                if (item == null) return new Response<WorkoutPlanItemVM>(null, "Item not found.", true);
                item.WorkoutPlanId = model.WorkoutPlanId;
                item.DayNumber = model.DayNumber;
                item.ExerciseName = model.ExerciseName;
                item.Sets = model.Sets;
                item.Reps = model.Reps;
                item.RestDuration = model.RestDuration;
                item.Equipment = model.Equipment;
                item.VideoUrl = model.VideoUrl;
                item.Notes = model.Notes;
                item.IsActive = model.IsActive;
                UnitOfWork.WorkoutPlanItems.Update(item);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<WorkoutPlanItemVM>(model, null, false);
                return new Response<WorkoutPlanItemVM>(null, "Failed to update item.", true);
            }
            catch (Exception ex)
            {
                return new Response<WorkoutPlanItemVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var item = await UnitOfWork.WorkoutPlanItems.GetByIdAsync(id);
                if (item == null) return new Response<bool>(false, "Item not found.", true);
                UnitOfWork.WorkoutPlanItems.Remove(item);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete item.", true);
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
                var item = await UnitOfWork.WorkoutPlanItems.GetByIdAsync(id);
                if (item == null) return new Response<bool>(false, "Item not found.", true);
                item.ToggleStatus();
                UnitOfWork.WorkoutPlanItems.Update(item);
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

