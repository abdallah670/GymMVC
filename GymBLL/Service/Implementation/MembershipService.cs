using AutoMapper;
using GymBLL.ModelVM.External;
using GymBLL.Service.Abstract;
using GymDAL.Entities;
using GymDAL.Repo.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBLL.Service.Implementation
{
    public class MembershipService : IMembershipService
    {
        public IMapper Mapper { get; }
        public IUnitOfWork UnitOfWork { get; }

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Response<MembershipVM>> CreateAsync(MembershipVM model)
        {
            try
            {
                var membership = Mapper.Map<Membership>(model);
                await UnitOfWork.Memberships.AddAsync(membership);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                {
                    model.Id = membership.Id;
                    return new Response<MembershipVM>(model, null, false);
                }
                return new Response<MembershipVM>(null, "Failed to create membership.", true);
            }
            catch (Exception ex)
            {
                return new Response<MembershipVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<MembershipVM>> GetByIdAsync(int id)
        {
            try
            {
                var membership = await UnitOfWork.Memberships.GetByIdAsync(id);
                if (membership != null)
                {
                    var vm = Mapper.Map<MembershipVM>(membership);
                    return new Response<MembershipVM>(vm, null, false);
                }
                return new Response<MembershipVM>(null, "Membership not found.", true);
            }
            catch (Exception ex)
            {
                return new Response<MembershipVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<MembershipVM>>> GetAllAsync()
        {
            try
            {
                var memberships = await UnitOfWork.Memberships.GetAllAsync();
                var vms = memberships.Select(m => Mapper.Map<MembershipVM>(m)).ToList();
                return new Response<List<MembershipVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MembershipVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<List<MembershipVM>>> GetActiveAsync()
        {
            try
            {
                var memberships = await UnitOfWork.Memberships.FindAsync(m => m.IsActive);
                var vms = memberships.Select(m => Mapper.Map<MembershipVM>(m)).ToList();
                return new Response<List<MembershipVM>>(vms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MembershipVM>>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<MembershipVM>> UpdateAsync(MembershipVM model)
        {
            try
            {
                var membership = await UnitOfWork.Memberships.GetByIdAsync(model.Id);
                if (membership == null)
                    return new Response<MembershipVM>(null, "Membership not found.", true);

                membership.MembershipType = model.MembershipType;
                membership.Price = model.Price;
                membership.DurationInMonths = model.DurationInMonths;
                membership.HasDietPlan = model.HasDietPlan;
                membership.HasWorkoutPlan = model.HasWorkoutPlan;
                membership.HasNotification = model.HasNotification;
                membership.IsFollowedByTrainer = model.IsFollowedByTrainer;
                membership.PreferredTrainingTime = model.PreferredTrainingTime;
                membership.TrainingIntensity = model.TrainingIntensity;
                
                membership.IsActive = model.IsActive;

                UnitOfWork.Memberships.Update(membership);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<MembershipVM>(model, null, false);
                return new Response<MembershipVM>(null, "Failed to update membership.", true);
            }
            catch (Exception ex)
            {
                return new Response<MembershipVM>(null, $"Error: {ex.Message}", true);
            }
        }

        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var membership = await UnitOfWork.Memberships.GetByIdAsync(id);
                if (membership == null)
                    return new Response<bool>(false, "Membership not found.", true);

                UnitOfWork.Memberships.Remove(membership);
                var result = await UnitOfWork.SaveAsync();
                
                if (result > 0)
                    return new Response<bool>(true, null, false);
                return new Response<bool>(false, "Failed to delete membership.", true);
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
                var membership = await UnitOfWork.Memberships.GetByIdAsync(id);
                if (membership == null)
                    return new Response<bool>(false, "Membership not found.", true);

                membership.ToggleStatus();
                UnitOfWork.Memberships.Update(membership);
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
