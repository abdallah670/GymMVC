











//using MenoDAL.Repo.Abstract;
//using Microsoft.AspNetCore.Mvc;
//using TestMVC.DAL.Entities;

//namespace MenoBLL.Service.Implementation
//{
//    public class AccountService : IAccountService
//    {
//        private readonly IEmployeeRepo _employeeRepo;
//        private readonly UserManager<Employee> _userManager;
//        private readonly SignInManager<Employee> _signInManager;
//        private readonly IMapper _mapper;

//        // Update the constructor to accept UserManager<Employee> as a parameter.
//        public AccountService(IEmployeeRepo employeeRepo, IMapper mapper,
//            UserManager<Employee> userManager, SignInManager<Employee> signInManager)
//        {
//            _employeeRepo = employeeRepo;
//            _mapper = mapper;
//            _userManager = userManager;
//            _signInManager = signInManager;
//        }

//        public async Task<IdentityResult> CreateEmployee(RegisterEmployeeVM employeeVM)
//        {
//            var User = new Employee(employeeVM.Name, employeeVM.Salary, "Meno", employeeVM.UserName);
//            try
//            {
//                var result = await _userManager.CreateAsync(User, employeeVM.Password);
//                if (result.Succeeded)
//                {
//                    var IshaveRole =await _userManager.IsInRoleAsync(User, "Admin");
//                    if (!IshaveRole)
//                    {
//                        var resultRole = await _userManager.AddToRoleAsync(User, "Admin");
//                    }

//                }

//                 return result;
               
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }


//        }

//        public async Task<Microsoft.AspNetCore.Identity.SignInResult> Login(LoginEmployeeVM employeeVM)
//        {
            
//            try
//            { 
//                var result = await _signInManager.PasswordSignInAsync(employeeVM.UserName, employeeVM.Password, true,false);

//                return result;

//            }
//            catch (Exception ex)
//            {
//                throw;
//            }

//        }
//        public async Task<bool> SignOut()
//        {
//            try
//            {
//                 await _signInManager.SignOutAsync();

//                return true;

//            }
//            catch (Exception ex)
//            {
//                throw;
//            }

//        }

      

//        // ... rest of the class remains unchanged ...
//    }
//}
