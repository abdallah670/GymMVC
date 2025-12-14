using GymBLL.ModelVM.User.AppUser;
using GymBLL.ModelVM.User.Member;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using GymBLL.ModelVM;
using System.Collections.Generic;

namespace GymBLL.Service.Abstract
{
    public interface IMemberService
    {
        Task<IdentityResult> Register(RegisterUserVM user);
        Task<IdentityResult> Register(MemberDetailsVM user);
        Task<Response<MemberVM>> GetMemberByIdAsync(string memberId);
        Task<Response<MemberVM>> GetMemberByEmailAsync(string email);
        Task<Response<MemberVM>> UpdateMemberAsync(MemberVM memberVm);
        Task<Response<bool>> DeleteMemberAsync(string memberId);
        Task<Response<List<MemberVM>>> GetAllMembersAsync();
        Task<Response<List<MemberVM>>> GetActiveMembersAsync();
        Task<Response<List<MemberVM>>> GetNotActiveMembersAsync();
        // Pagination
        Task<Response<PagedResult<MemberVM>>> GetPagedMembersAsync(int pageNumber, int pageSize);
        Task<Response<MemberVM>> CompleteProfileAsync(MemberProfileVM model);
        Task<bool> HasCompletedProfileAsync(string memberId);
    }
}
