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
    public class WorkoutAssignmentService : IWorkoutAssignmentService
    {
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public WorkoutAssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Response<WorkoutAssignmentVM>> CreateAsync(WorkoutAssignmentVM model)
        {
            try
            {
                var assignment = Mapper.Map<WorkoutAssignment>(model);
                assignment.WorkoutPlanId = model.WorkoutPlanId;
                await UnitOfWork.WorkoutAssignments.AddAsync(assignment);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    model.Id = assignment.Id;
                    return new Response<WorkoutAssignmentVM>(model, null, false);
                }
                return new Response<WorkoutAssignmentVM>(null, "Failed to create assignment.", true);
            }
            catch (Exception ex)
            {
                return new Response<WorkoutAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<WorkoutAssignmentVM>> GetByIdAsync(int id)
        {
            try
            {
                var assignment = await UnitOfWork.WorkoutAssignments.GetByIdAsync(id);
                if (assignment != null)
                {
                    var vm = Mapper.Map<WorkoutAssignmentVM>(assignment);
                    return new Response<WorkoutAssignmentVM>(vm, null, false);
                }
                return new Response<WorkoutAssignmentVM>(null, "Assignment not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<WorkoutAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<WorkoutAssignmentVM>>> GetAllAsync()
        {
            try
            {
                var assignments = await UnitOfWork.WorkoutAssignments.GetAllAsync();
                var vms = assignments.Select(a => Mapper.Map<WorkoutAssignmentVM>(a)).ToList();
                return new Response<List<WorkoutAssignmentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutAssignmentVM>>(null, $"Error: {ex.Message}", true);
            }
        }


        public async Task<Response<List<WorkoutAssignmentVM>>> GetByWorkoutPlanIdAsync(int workoutPlanId)
        {
            try
            {
                var assignments = await UnitOfWork.WorkoutAssignments.FindAsync(a => a.WorkoutPlanId == workoutPlanId);
                var vms = assignments.Select(a => Mapper.Map<WorkoutAssignmentVM>(a)).ToList();
                return new Response<List<WorkoutAssignmentVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<WorkoutAssignmentVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<WorkoutAssignmentVM>> UpdateAsync(WorkoutAssignmentVM model)
        {
            try
            {
                var assignment = await UnitOfWork.WorkoutAssignments.GetByIdAsync(model.Id);
                if (assignment == null) return new Response<WorkoutAssignmentVM>(null, "Assignment not found.", true);
                
                assignment.WorkoutPlanId = model.WorkoutPlanId;
                assignment.StartDate = model.StartDate;
                assignment.EndDate = model.EndDate;
                assignment.IsActive = model.IsActive;
                
                UnitOfWork.WorkoutAssignments.Update(assignment);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0) return new Response<WorkoutAssignmentVM>(model, null, false);
                return new Response<WorkoutAssignmentVM>(null, "Failed to update assignment.", true);
            }
            catch (Exception ex)
            {
                return new Response<WorkoutAssignmentVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var assignment = await UnitOfWork.WorkoutAssignments.GetByIdAsync(id);
                if (assignment == null) return new Response<bool>(false, "Assignment not found.", true);
                
                UnitOfWork.WorkoutAssignments.Remove(assignment);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0) return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete assignment.", true);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error: {ex.Message}", true);
            }
        }

      
    }
}
