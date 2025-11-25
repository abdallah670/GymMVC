





using GymDAL.Mappper;
using GymDAL.Repo.Abstract.Extra;

using GymDAL.Repo.Abstract.Nutrition;
using GymDAL.Repo.Abstract.Users;
using GymDAL.Repo.Abstract.Workout;
using GymDAL.Repo.Implementation;

namespace MenoDAL.Commnon
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
