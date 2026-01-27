





using GymDAL.Mappper;
using GymDAL.Repo.Abstract.Financial;
using GymDAL.Repo.Abstract.Core;
using GymDAL.Repo.Abstract.Communication;

using GymDAL.Repo.Abstract.Nutrition;
using GymDAL.Repo.Abstract.Users;
using GymDAL.Repo.Abstract.Workout;
using GymDAL.Repo.Implementation;
using GymDAL.Repo.Implementation.Financial;
using GymDAL.Repo.Implementation.Workout;
using GymDAL.Repo.Implementation.Communication;
using GymDAL.Repo.Implementation.Core;
using GymDAL.Repo.Implementation.Users;
using GymDAL.Repo.Implementation.Nutrition;

namespace GymDAL.Common
{
    public static class ModularDataAccessLayer
    {
        public static IServiceCollection AddModularDataAccessLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(new DomainProfile()));
            // User-related repositories
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
          
            services.AddScoped<ITrainerRepository, TrainerRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();

            // Workout-related repositories
            services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>();
            services.AddScoped<IWorkoutAssignmentRepository, WorkoutAssignmentRepository>();
         
            services.AddScoped<IWorkoutPlanItemRepository, WorkoutPlanItemRepository>();

            // Nutrition-related repositories
            services.AddScoped<IDietPlanRepository, DietPlanRepository>();
            services.AddScoped<IDietPlanAssignmentRepository, DietPlanAssignmentRepository>();
            services.AddScoped<IDietPlanItemRepository, DietPlanItemRepository>();
            services.AddScoped<IMealLogRepository, MealLogRepository>();


            // Extra repositories
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
         

            // Add Unit of Work if you have it
             services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
    }
}
