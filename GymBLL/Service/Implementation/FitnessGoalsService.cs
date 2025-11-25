using GymBLL.ModelVM.External;
using GymBLL.Service.Abstract;
using GymDAL.Entities.External;

using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GymBLL.Service.Implementation
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
                return new Response<FitnessGoalsVM>(resultVm,null,false);
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
                return new Response<List<FitnessGoalsVM>>(null, $"Error retrieving fitness goals: {ex.Message}", false);;
            }
        }

        public Task<Response<List<FitnessGoalsVM>>> GetActiveFitnessGoalsAsync()
        {
            try
            {
                var fitnessGoals =  _unitOfWork.FitnessGoalsRepository.FindAsync(fg => fg.IsActive);
                var fitnessGoalsVm = _mapper.Map<List<FitnessGoalsVM>>(fitnessGoals);
                return Task.FromResult(new Response<List<FitnessGoalsVM>>(fitnessGoalsVm, null, false));
            }
            catch (Exception ex)
            {
                return Task.FromResult(new Response<List<FitnessGoalsVM>>(null, $"Error retrieving active fitness goals: {ex.Message}", false));
            }

        }
    }
}