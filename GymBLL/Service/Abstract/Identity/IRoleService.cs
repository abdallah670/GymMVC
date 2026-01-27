using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM;
using GymBLL.Response;
using System.Threading.Tasks;

namespace GymBLL.Service.Abstract.Identity
{
    public interface IRoleService
    {
        //Response<List<GetEmployeeVM>> GetNotActiveEmployee();
        //Response<List<GetEmployeeVM>> GetActiveEmployee();
        Task<bool> CreateRole(RoleVM Role);
    }
}
