
using GymBLL.Service.Abstract;
using GymBLL.Mappper;
using GymBLL.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace GymBLL.Commnon
{
    public static class ModularBusinessAccessLayer
    {
        public static IServiceCollection AddModularBusinessLogicLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(new DomainProfile()));
            
            // User Services
            services.AddScoped<ITrainerService, Trainerservice>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMemberService, MemberService>();

            // Workout Services
            services.AddScoped<IWorkoutPlanService, WorkoutPlanService>();
            services.AddScoped<IWorkoutAssignmentService, WorkoutAssignmentService>();
            services.AddScoped<IWorkoutPlanItemService, WorkoutPlanItemService>();

            // Nutrition Services
            services.AddScoped<IDietPlanService, DietPlanService>();
            services.AddScoped<IDietPlanAssignmentService, DietPlanAssignmentService>();
            services.AddScoped<IDietPlanItemService, DietPlanItemService>();
            services.AddScoped<IMealLogService, MealLogService>();

            // Other Services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IMembershipService, MembershipService>();
           services.AddScoped<IFitnessGoalsService, FitnessGoalsService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
