using GymBLL.Service.Abstract.Identity;
using GymBLL.Service.Abstract.Member;
using GymBLL.Service.Abstract.Trainer;
using GymBLL.Service.Abstract.Workout;
using GymBLL.Service.Abstract.Nutrition;
using GymBLL.Service.Abstract.Communication;
using GymBLL.Service.Abstract.Financial;
using GymBLL.Service.Implementation.Identity;
using GymBLL.Service.Implementation.Member;
using GymBLL.Service.Implementation.Trainer;
using GymBLL.Service.Implementation.Workout;
using GymBLL.Service.Implementation.Nutrition;
using GymBLL.Service.Implementation.Communication;
using GymBLL.Service.Implementation.Financial;
using GymBLL.Service.Implementation.Trainer;
using GymBLL.Service.Abstract;
using GymBLL.Service.Implementation;
using GymBLL.Mappper;
using Microsoft.Extensions.DependencyInjection;

namespace GymBLL.Common
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
            services.AddScoped<ITempRegistrationService, TempRegistrationService>();
            services.AddScoped<IStripePaymentService, StripePaymentService>();

            return services;
        }
    }
}
