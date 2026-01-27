using GymBLL.ModelVM.Member;
using GymBLL.ModelVM;
using GymBLL.Response;
using GymBLL.ModelVM.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GymBLL.Service.Abstract.Member
{
    public interface IMemberService
    {
       // Task<IdentityResult> Register(RegisterUserVM user);
        Task<IdentityResult> Register(MemberProfileVM user);
        Task<Response<MemberVM>> GetMemberByIdAsync(string memberId);
        Task<Response<MemberVM>> GetMemberByEmailAsync(string email);
        Task<Response<MemberVM>> UpdateMemberAsync(MemberVM memberVm);
        Task<Response<bool>> DeleteMemberAsync(string memberId);
        Task<Response<List<MemberVM>>> GetAllMembersAsync();
        Task<Response<List<MemberVM>>> GetActiveMembersAsync();
        Task<Response<List<MemberVM>>> GetNotActiveMembersAsync();
        
        // Pagination
        Task<Response<PagedResult<MemberVM>>> GetPagedMembersAsync(int pageNumber, int pageSize);
        Task<Response<MemberVM>> AddUserProfileAsync(MemberProfileVM model);
        Task<bool> HasCompletedProfileAsync(string memberId);
    
    }
}
