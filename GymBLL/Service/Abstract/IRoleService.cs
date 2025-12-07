





namespace MenoBLL.Service.Abstract
{
    public interface IRoleService
    {
        //Response<List<GetEmployeeVM>> GetNotActiveEmployee();
        //Response<List<GetEmployeeVM>> GetActiveEmployee();
         Task<bool> CreateRole(RoleVM Role);
       




    }
}
