using GymBLL.Response;
using GymBLL.ModelVM;
using GymBLL.ModelVM.Identity;
using GymBLL.Service.Abstract.Identity;
namespace GymBLL.Service.Implementation.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<bool> CreateRole(RoleVM RoleVM)
        {
            try
            {
                var getRoleByName = await _roleManager.FindByNameAsync(RoleVM.Name);
                if (getRoleByName is not
                    {
                    })
                {
                    var Role = _mapper.Map<IdentityRole>(RoleVM);
                    var result = await _roleManager.CreateAsync(Role);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

