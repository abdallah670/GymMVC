





namespace MenoBLL.Service.Abstract
{
    public interface IAccountService
    {
        //Response<List<GetEmployeeVM>> GetNotActiveEmployee();
        //Response<List<GetEmployeeVM>> GetActiveEmployee();
         Task<IdentityResult> CreateEmployee(RegisterEmployeeVM employee);
         Task<SignInResult> Login(LoginEmployeeVM employee);
         Task<bool> SignOut();





    }
}
