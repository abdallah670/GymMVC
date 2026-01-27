using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Member;
using GymBLL.Service.Abstract.Member;
using GymBLL.ModelVM.Financial;
using GymBLL.Service.Abstract;
using GymDAL.Entities.Core;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace GymBLL.Service.Implementation.Member
{
    public class FitnessGoalsService : IFitnessGoalsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FitnessGoalsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Response<FitnessGoalsVM>> CreateFitnessGoalAsync(FitnessGoalsVM fitnessGoalsVm)
        {
            try
            {
                var fitnessGoal = _mapper.Map<FitnessGoals>(fitnessGoalsVm);
                fitnessGoal.CreatedAt = DateTime.UtcNow;
                fitnessGoal.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.FitnessGoalsRepository.AddAsync(fitnessGoal);
                await _unitOfWork.SaveAsync();
                var resultVm = _mapper.Map<FitnessGoalsVM>(fitnessGoal);
                return new Response<FitnessGoalsVM>(resultVm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<FitnessGoalsVM>(null, $"Error creating fitness goal: {ex.Message}", false);
            }
        }
        public async Task<Response<List<FitnessGoalsVM>>> GetAllFitnessGoalsAsync()
        {
            try
            {
                var fitnessGoals = await _unitOfWork.FitnessGoalsRepository.GetAllAsync();
                var fitnessGoalsVm = _mapper.Map<List<FitnessGoalsVM>>(fitnessGoals);
                return new Response<List<FitnessGoalsVM>>(fitnessGoalsVm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<FitnessGoalsVM>>(null, $"Error retrieving fitness goals: {ex.Message}", false);
                ;
            }
        }
        public Task<Response<List<FitnessGoalsVM>>> GetActiveFitnessGoalsAsync()
        {
            try
            {
                var fitnessGoals = _unitOfWork.FitnessGoalsRepository.FindAsync(fg => fg.IsActive);
                var fitnessGoalsVm = _mapper.Map<List<FitnessGoalsVM>>(fitnessGoals);
                return Task.FromResult(new Response<List<FitnessGoalsVM>>(fitnessGoalsVm, null, false));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new Response<List<FitnessGoalsVM>>(null, $"Error retrieving active fitness goals: {ex.Message}", false));
            }
        }
        public async Task<Response<FitnessGoalsVM>> GetByIdAsync(int id)
        {
            try
            {
                var fitnessGoal = await _unitOfWork.FitnessGoalsRepository.GetByIdAsync(id);
                if (fitnessGoal == null)
                    return new Response<FitnessGoalsVM>(null, "Fitness Goal not found", true);

                var fitnessGoalVm = _mapper.Map<FitnessGoalsVM>(fitnessGoal);
                return new Response<FitnessGoalsVM>(fitnessGoalVm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<FitnessGoalsVM>(null, $"Error retrieving fitness goal: {ex.Message}", true);
            }
        }

        public async Task<Response<FitnessGoalsVM>> UpdateFitnessGoalAsync(FitnessGoalsVM fitnessGoalsVm)
        {
            try
            {
                var existingGoal = await _unitOfWork.FitnessGoalsRepository.GetByIdAsync(fitnessGoalsVm.Id.Value);
                if (existingGoal == null)
                    return new Response<FitnessGoalsVM>(null, "Fitness Goal not found", true);

                _mapper.Map(fitnessGoalsVm, existingGoal);
                existingGoal.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.FitnessGoalsRepository.Update(existingGoal);
                await _unitOfWork.SaveAsync();

                var resultVm = _mapper.Map<FitnessGoalsVM>(existingGoal);
                return new Response<FitnessGoalsVM>(resultVm, null, false);
            }
            catch (Exception ex)
            {
                return new Response<FitnessGoalsVM>(null, $"Error updating fitness goal: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteFitnessGoalAsync(int id)
        {
            try
            {
                var existingGoal = await _unitOfWork.FitnessGoalsRepository.GetByIdAsync(id);
                if (existingGoal == null)
                    return new Response<bool>(false, "Fitness Goal not found", true);

                _unitOfWork.FitnessGoalsRepository.Remove(existingGoal);
                await _unitOfWork.SaveAsync();

                return new Response<bool>(true, null, false);
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Error deleting fitness goal: {ex.Message}", true);
            }
        }
    }
}

