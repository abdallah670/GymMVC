using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Nutrition;
using GymBLL.Service.Abstract.Nutrition;
using AutoMapper;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Nutrition;
using GymDAL.Entities.Workout;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Add this at the top if not already present
using Microsoft.EntityFrameworkCore.Query; // Add this at the top if not already present

namespace GymBLL.Service.Implementation.Nutrition
{
    public class DietPlanAssignmentService : IDietPlanAssignmentService
    {
        public IMapper Mapper
        {
            get;
        }
        public IUnitOfWork UnitOfWork
        {
            get;
        }
        public DietPlanAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, IDietPlanService dietPlanService)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        public async Task<Response<DietPlanAssignmentVM>> CreateAsync(DietPlanAssignmentVM model)
        {
            try
            {
                var assignment = Mapper.Map<DietPlanAssignment>(model);

                // Just set the foreign key, do NOT create a new DietPlan
                assignment.DietPlanId = model.DietPlanId;

                // REMOVE THIS LINE: assignment.DietPlan = Mapper.Map<DietPlan>(model.DietPlan);
                await UnitOfWork.DietPlanAssignments.AddAsync(assignment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    model.Id = assignment.Id;
                    return new Response<DietPlanAssignmentVM>(model, null, false);
                }
                return new Response<DietPlanAssignmentVM>(null, "Failed to create assignment.", true);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<DietPlanAssignmentVM>> GetByIdAsync(int id)
        {
            try
            {
                var assignment = await UnitOfWork.DietPlanAssignments.GetByIdAsync(id);
                if (assignment != null)
                {
                    var vm = Mapper.Map<DietPlanAssignmentVM>(assignment);
                    return new Response<DietPlanAssignmentVM>(vm, null, false);
                }
                return new Response<DietPlanAssignmentVM>(null, "Assignment not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<DietPlanAssignmentVM>>> GetAllAsync()
        {
            try
            {
                var assignments = await UnitOfWork.DietPlanAssignments.GetAllAsync();
                var vms = assignments.Select(a => Mapper.Map<DietPlanAssignmentVM>(a)).ToList();
                return new Response<List<DietPlanAssignmentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<DietPlanAssignmentVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<List<DietPlanAssignmentVM>>> GetByDietPlanIdAsync(int dietPlanId)
        {
            try
            {
                var assignments = await UnitOfWork.DietPlanAssignments.FindAsync(a => a.DietPlanId == dietPlanId);
                var vms = assignments.Select(a => Mapper.Map<DietPlanAssignmentVM>(a)).ToList();
                return new Response<List<DietPlanAssignmentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<DietPlanAssignmentVM>>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<DietPlanAssignmentVM>> UpdateAsync(DietPlanAssignmentVM model)
        {
            try
            {
                var assignment = await UnitOfWork.DietPlanAssignments.GetByIdAsync(model.Id);
                if (assignment == null) return new Response<DietPlanAssignmentVM>(null, "Assignment not found.", true);
                assignment.DietPlanId = model.DietPlanId;
                assignment.StartDate = model.StartDate;
                assignment.EndDate = model.EndDate;
                assignment.IsActive = model.IsActive;
                UnitOfWork.DietPlanAssignments.Update(assignment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<DietPlanAssignmentVM>(model, null, false);
                return new Response<DietPlanAssignmentVM>(null, "Failed to update assignment.", true);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var assignment = await UnitOfWork.DietPlanAssignments.GetByIdAsync(id);
                if (assignment == null) return new Response<bool>(false, "Assignment not found.", true);
                UnitOfWork.DietPlanAssignments.Remove(assignment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete assignment.", true);
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
                var assignment = await UnitOfWork.DietPlanAssignments.GetByIdAsync(id);
                if (assignment == null) return new Response<bool>(false, "Assignment not found.", true);
                assignment.ToggleStatus();
                UnitOfWork.DietPlanAssignments.Update(assignment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to toggle status.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<DietPlanAssignmentVM>> GetLastWorkoutForMemberAsync(int DietAssignmentID)
        {
            try
            {
                var assignment =await UnitOfWork.DietPlanAssignments.Get(
                    filter: x => x.Id == DietAssignmentID,
                    include: q => q.Include(x => x.DietPlan)   
                ).FirstOrDefaultAsync();
                
                if (assignment == null) return new Response<DietPlanAssignmentVM>(null, "No workout assignment found.", true);
                var vm = Mapper.Map<DietPlanAssignmentVM>(assignment);
                return new Response<DietPlanAssignmentVM>(vm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<DietPlanAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }
    }
}