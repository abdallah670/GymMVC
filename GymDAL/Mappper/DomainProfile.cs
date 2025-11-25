



namespace GymDAL.Mappper
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<ApplicationUser , Admin>().ReverseMap();
            CreateMap<ApplicationUser, Trainer>().ReverseMap();
            CreateMap<ApplicationUser , Member>().ReverseMap();
            CreateMap<WorkoutPlan, WorkoutPlan>();
            CreateMap<WorkoutAssignment, WorkoutAssignment>();
            CreateMap<WorkoutLog, WorkoutLog>();
            CreateMap<WorkoutPlanItem, WorkoutPlanItem>();
            // Nutrition-related mappings
            CreateMap<DietPlan, DietPlan>();
            CreateMap<DietPlanAssignment, DietPlanAssignment>();
            CreateMap<DietPlanItem, DietPlanItem>();
            CreateMap<MealLog, MealLog>();

            // Log-related mappings
            CreateMap<ProgressLog, ProgressLog>();
            CreateMap<ReportLog, ReportLog>();
            CreateMap<SystemLog, SystemLog>();

            // Extra-related mappings
            CreateMap<Membership, Membership>();
            CreateMap<Notification, Notification>();
            CreateMap<Payment, Payment>();
            CreateMap<TrainerAvailability, TrainerAvailability>();
            CreateMap<TrainingSession, TrainingSession>();


        }
    }
}
