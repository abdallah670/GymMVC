using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Member;
using GymBLL.Service.Abstract.Member;
using AutoMapper;
using GymBLL.ModelVM.Financial;
using GymBLL.ModelVM.Identity;
using GymBLL.Service.Abstract;
using  GymDAL.Entities.Users;
using GymDAL.Repo.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GymBLL.Service.Implementation.Member
{
    public class MemberService : IMemberService
    {
        public IMapper Mapper
        {
            get;
        }
        public IUnitOfWork UnitOfWork
        {
            get;
        }
        private readonly UserManager<ApplicationUser> _userManager;
        public MemberService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
        async Task<IdentityResult> Register(RegisterUserVM user)
        {
            try
            {
                var member = Mapper.Map<GymDAL.Entities.Users.Member>(user);
                member.UserName = user.Email;
                member.EmailConfirmed = true;
                member.JoinDate = DateTime.UtcNow;
                var result = await _userManager.CreateAsync(member, user.Password);
                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(member, "Member");
                    if (!roleResult.Succeeded)
                    {
                        return IdentityResult.Failed(roleResult.Errors.ToArray());
                    }
                }
                await UnitOfWork.CommitTransactionAsync();
                return result;
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                throw;
            }
        }
        public async Task<Response<MemberVM>> GetMemberByIdAsync(string memberId)
        {
            try
            {
                var member = await UnitOfWork.Members.GetByIdAsync(memberId);
                if (member != null)
                {
                    var memberVm = Mapper.Map<MemberVM>(member);
                    memberVm.FitnessGoal = new FitnessGoalsVM();
                    memberVm.FitnessGoal.GoalName = member.FitnessGoal?.GoalName;
                    memberVm.FitnessGoal.GoalDescription = member.FitnessGoal?.GoalDescription;
                    memberVm.FitnessGoal.Id = member.FitnessGoal?.Id;
                    return new Response<MemberVM>(memberVm, null, false);
                }
                else
                {
                    return new Response<MemberVM>(null, "Member not found.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<MemberVM>(null, $"Failed to get member: {ex.Message}", true);
            }
        }
        public async Task<Response<MemberVM>> GetMemberByEmailAsync(string email)
        {
            try
            {
                var member = await UnitOfWork.Members.FirstOrDefaultAsync(m => m.Email == email);
                if (member != null)
                {
                    var memberVm = Mapper.Map<MemberVM>(member);
                    return new Response<MemberVM>(memberVm, null, false);
                }
                else
                {
                    return new Response<MemberVM>(null, "Member not found.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<MemberVM>(null, $"Failed to get member: {ex.Message}", true);
            }
        }
        public async Task<Response<MemberVM>> UpdateMemberAsync(MemberVM memberVm)
        {
            try
            {
                var existingMember = await UnitOfWork.Members.GetByIdAsync(memberVm.Id);
                if (existingMember == null)
                {
                    return new Response<MemberVM>(null, "Member not found.", true);
                }
                existingMember.FullName = memberVm.FullName;
                existingMember.Phone = memberVm.Phone;
                existingMember.ProfilePicture = memberVm.ProfilePicture;
                existingMember.CurrentWeight = (double)memberVm.CurrentWeight;
                existingMember.Height = (double)memberVm.Height;
                existingMember.Email = memberVm.Email;
                existingMember.Gender = memberVm.Gender;
                UnitOfWork.Members.Update(existingMember);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<MemberVM>(memberVm, null, false);
                }
                else
                {
                    return new Response<MemberVM>(null, "Failed to update member.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<MemberVM>(null, $"Failed to update member: {ex.Message}", true);
            }
        }
        public async Task<Response<bool>> DeleteMemberAsync(string memberId)
        {
            try
            {
                var member = await UnitOfWork.Members.GetByIdAsync(memberId);
                if (member == null)
                {
                    return new Response<bool>(false, "Member not found.", true);
                }

                // [NEW] Cancel active subscription if exists
                var subscriptions = await UnitOfWork.Subscriptions.FindAsync(s => s.MemberId == memberId && s.Status == GymDAL.Enums.SubscriptionStatus.Active);
                var activeSubscription = subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();
                if (activeSubscription != null)
                {
                    activeSubscription.Status = GymDAL.Enums.SubscriptionStatus.Cancelled;
                    activeSubscription.EndDate = DateTime.UtcNow; // Terminate effective immediately
                    UnitOfWork.Subscriptions.Update(activeSubscription);
                }

                member.IsActive = false;
                UnitOfWork.Members.Update(member);
                var result = await UnitOfWork.SaveAsync();
                if (result > 0)
                {
                    return new Response<bool>(true, null, false);
                }
                else
                {
                    return new Response<bool>(false, "Failed to delete member.", true);
                }
            }
            catch (Exception ex)
            {
                return new Response<bool>(false, $"Failed to delete member: {ex.Message}", true);
            }
        }
        public async Task<Response<List<MemberVM>>> GetAllMembersAsync()
        {
            try
            {
                var members = await UnitOfWork.Members.GetAllAsync();
                var memberVms = members.Select(m => new MemberVM
                {
                    Id = m.Id,
                    Email = m.Email,
                    FullName = m.FullName,
                    Phone = m.Phone,
                    ProfilePicture = m.ProfilePicture,
                    JoinDate = m.JoinDate,
                    Height = m.Height,
                    CurrentWeight = m.CurrentWeight,
                    Gender = m.Gender,
                    FitnessGoal = new FitnessGoalsVM
                    {
                        GoalName = m.FitnessGoal?.GoalName,
                        Id = m.FitnessGoal?.Id,
                        GoalDescription = m.FitnessGoal?.GoalDescription
                    }
                }).ToList();
                return new Response<List<MemberVM>>(memberVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MemberVM>>(null, $"Failed to get members: {ex.Message}", true);
            }
        }
        public async Task<IdentityResult> Register(MemberProfileVM user)
        {
            try
            {
                var member = Mapper.Map<RegisterUserVM>(user);
                // Use await here instead of .Result
                var result = await Register(member);

                if (result.Succeeded)
                {



                    var completeResult = await AddUserProfileAsync(user);
                    if (completeResult != null && !completeResult.ISHaveErrorOrnNot)
                    {
                        return IdentityResult.Success;
                    }
                    else
                    {
                        return IdentityResult.Failed(new IdentityError { Description = "Failed to complete member profile." });
                    }

                }

                else
                {
                    return IdentityResult.Failed(result.Errors.ToArray());
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                throw;
            }
        }
        public async Task<Response<MemberVM>> AddUserProfileAsync(MemberProfileVM model)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var member = await UnitOfWork.Members.GetByEmailAsync(model.Email);
                if (member == null) return new Response<MemberVM>(null, "Failed to get member", true);
                // Update properties of the EXISTING tracked entity
                member.Age = model.Age;
                member.CurrentWeight = model.CurrentWeight;
                member.Height = model.Height;
                member.Gender = model.Gender;
                member.FitnessGoalId = model.FitnessGoalId;
                member.ActivityLevel = model.ActivityLevel;
                member.HasCompletedProfile = true;
                // Mark/Ensure it is updated (though tracking usually handles this)
                UnitOfWork.Members.Update(member);
                await UnitOfWork.CommitTransactionAsync();
                var memberVM = Mapper.Map<MemberVM>(member);
                return new Response<MemberVM>(memberVM, null, false);
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                return new Response<MemberVM>(null, $"Failed to complete profile: {ex.Message}", true);
            }
        }
        public async Task<bool> HasCompletedProfileAsync(string memberId)
        {
            var member = await UnitOfWork.Members.GetByIdAsync(memberId);
            return member?.HasCompletedProfile ?? false;
        }
        public async Task<Response<List<MemberVM>>> GetActiveMembersAsync()
        {
            try
            {
                var members = await UnitOfWork.Members.FindAsync(m => m.IsActive);
                var memberVms = members.Select(m => new MemberVM
                {
                    Id = m.Id,
                    Email = m.Email,
                    FullName = m.FullName,
                    Phone = m.Phone,
                    ProfilePicture = m.ProfilePicture,
                    JoinDate = m.JoinDate,
                    Height = m.Height,
                    CurrentWeight = m.CurrentWeight,
                    Gender = m.Gender,
                    FitnessGoal = new FitnessGoalsVM
                    {
                        GoalName = m.FitnessGoal.GoalName,
                        Id = m.FitnessGoal.Id,
                        GoalDescription = m.FitnessGoal.GoalDescription
                    }
                }).ToList();
                return new Response<List<MemberVM>>(memberVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MemberVM>>(null, $"Failed to get members: {ex.Message}", true);
            }
        }
        public async Task<Response<List<MemberVM>>> GetNotActiveMembersAsync()
        {
            try
            {
                var members = await UnitOfWork.Members.FindAsync(m => !m.IsActive);
                var memberVms = members.Select(m => new MemberVM
                {
                    Id = m.Id,
                    Email = m.Email,
                    FullName = m.FullName,
                    Phone = m.Phone,
                    ProfilePicture = m.ProfilePicture,
                    JoinDate = m.JoinDate,
                    Height = m.Height,
                    CurrentWeight = m.CurrentWeight,
                    Gender = m.Gender,
                    FitnessGoal = new FitnessGoalsVM
                    {
                        GoalName = m.FitnessGoal.GoalName,
                        Id = m.FitnessGoal.Id,
                        GoalDescription = m.FitnessGoal.GoalDescription
                    }
                }).ToList();
                return new Response<List<MemberVM>>(memberVms, null, false);
            }
            catch (Exception ex)
            {
                return new Response<List<MemberVM>>(null, $"Failed to get not active members: {ex.Message}", true);
            }
        }
        public async Task<Response<PagedResult<MemberVM>>> GetPagedMembersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var members = await UnitOfWork.Members.GetPagedAsync(pageNumber, pageSize);
                var totalCount = await UnitOfWork.Members.CountAsync();
                // Assuming generic repo has CountAsync
                var memberVms = members.Select(m => new MemberVM
                {


                    Id = m.Id,
                    Email = m.Email,
                    FullName = m.FullName,
                    Phone = m.Phone,
                    ProfilePicture = m.ProfilePicture,
                    JoinDate = m.JoinDate,
                    Height = m.Height,
                    CurrentWeight = m.CurrentWeight,
                    Gender = m.Gender,
                    FitnessGoal = new FitnessGoalsVM
                    {
                        GoalName = m.FitnessGoal?.GoalName,
                        Id = m.FitnessGoal?.Id,
                        GoalDescription = m.FitnessGoal?.GoalDescription
                    }
                }).ToList();
                var pagedResult = new PagedResult<MemberVM>(memberVms, totalCount, pageNumber, pageSize);
                return new Response<PagedResult<MemberVM>>(pagedResult, null, false);
            }
            catch (Exception ex)
            {
                return new Response<PagedResult<MemberVM>>(null, $"Failed to get paged members: {ex.Message}", true);
            }
        }

     
    }
}









