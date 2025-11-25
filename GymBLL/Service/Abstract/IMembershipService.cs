using GymBLL.ModelVM.External;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract
{
    public interface IMembershipService
    {
        Task<Response<MembershipVM>> CreateAsync(MembershipVM model);
        Task<Response<MembershipVM>> GetByIdAsync(int id);
      
        Task<Response<List<MembershipVM>>> GetAllAsync();
        Task<Response<List<MembershipVM>>> GetActiveAsync();
        Task<Response<MembershipVM>> UpdateAsync(MembershipVM model);
        Task<Response<bool>> DeleteAsync(int id);
        Task<Response<bool>> ToggleStatusAsync(int id);
    }
}
