
using MenoBLL.Mappper;

namespace MenoBLL.Commnon
{
    public static class ModularBusinessAccessLayer
    {
        public static IServiceCollection AddModularBusinessLogicLayer(this IServiceCollection services)
        {
            
            
            //services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddAutoMapper(x=>x.AddProfile(new DomainProfile()));
            //services.AddScoped<IDepartmentService, DepartmentService>();
            //services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IRoleService, RoleService>();

            //services.AddScoped<IProjectService, ProjectService>();
            return services;
        }
    }
}
