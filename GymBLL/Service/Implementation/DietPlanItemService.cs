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
    public class DietPlanItemService : IDietPlanItemService
    {
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public DietPlanItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Response<DietPlanItemVM>> CreateAsync(DietPlanItemVM model)
        {
            try
            {
                var item = Mapper.Map<DietPlanItem>(model);
                await UnitOfWork.DietPlanItems.AddAsync(item);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                {
                    model.Id = item.Id;
                    return new Response<DietPlanItemVM>(model, null, false);
                }
                return new Response<DietPlanItemVM>(null, "Failed to create item.", true);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanItemVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<DietPlanItemVM>> GetByIdAsync(int id)
        {
            try
            {
                var item = await UnitOfWork.DietPlanItems.GetByIdAsync(id);
                if (item != null)
                {
                    var vm = Mapper.Map<DietPlanItemVM>(item);
                    return new Response<DietPlanItemVM>(vm, null, false);
                }
                return new Response<DietPlanItemVM>(null, "Item not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanItemVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<DietPlanItemVM>>> GetAllAsync()
        {
            try
            {
                var items = await UnitOfWork.DietPlanItems.GetAllAsync();
                var vms = items.Select(i => Mapper.Map<DietPlanItemVM>(i)).ToList();
                return new Response<List<DietPlanItemVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<DietPlanItemVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<DietPlanItemVM>>> GetByDietPlanIdAsync(int dietPlanId)
        {
            try
            {
                var items = await UnitOfWork.DietPlanItems.FindAsync(i => i.DietPlanId == dietPlanId);
                var vms = items.Select(i => Mapper.Map<DietPlanItemVM>(i)).ToList();
                return new Response<List<DietPlanItemVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<DietPlanItemVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<DietPlanItemVM>> UpdateAsync(DietPlanItemVM model)
        {
            try
            {
                var item = await UnitOfWork.DietPlanItems.GetByIdAsync(model.Id);
                if (item == null)
                    return new Response<DietPlanItemVM>(null, "Item not found.", true);

                item.DietPlanId = model.DietPlanId;
                item.DayNumber = model.DayNumber;
                item.MealType = model.MealType;
                item.MealName = model.MealName;
                item.Calories = model.Calories;
                item.ProteinMacros = model.ProteinMacros;
                item.CarbMacros = model.CarbMacros;
                item.FatMacros = model.FatMacros;
                item.Ingredients = model.Ingredients;
                item.Notes = model.Notes;
                item.IsActive = model.IsActive;

                UnitOfWork.DietPlanItems.Update(item);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<DietPlanItemVM>(model, null, false);
                return new Response<DietPlanItemVM>(null, "Failed to update item.", true);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanItemVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var item = await UnitOfWork.DietPlanItems.GetByIdAsync(id);
                if (item == null)
                    return new Response<bool>(false, "Item not found.", true);

                UnitOfWork.DietPlanItems.Remove(item);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<bool>(true, null, false);
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
                var item = await UnitOfWork.DietPlanItems.GetByIdAsync(id);
                if (item == null)
                    return new Response<bool>(false, "Item not found.", true);

                item.ToggleStatus();
                UnitOfWork.DietPlanItems.Update(item);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to toggle status.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }
    }
}
