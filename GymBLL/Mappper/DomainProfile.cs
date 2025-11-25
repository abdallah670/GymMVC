using AutoMapper;
using GymBLL.ModelVM.User.AppUser;
using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.User.Trainer;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Notification;
using GymBLL.ModelVM.External;
using GymDAL.Entities.Users;
using GymDAL.Entities.Workout;
using GymDAL.Entities.Nutrition;
using GymDAL.Entities.Progress_and_Notification;
using GymDAL.Entities.External;
using GymDAL.Entities;
using MenoBLL.ModelVM.AccountVM;

namespace GymBLL.Mappper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<UserVM, ApplicationUser>().ReverseMap();
            CreateMap<RegisterUserVM, ApplicationUser>().ReverseMap();

            // Member mappings
            CreateMap<MemberDetailsVM, Member>().ReverseMap();
            
            CreateMap<Member, MemberVM>().ReverseMap();
            CreateMap<RegisterUserVM, Member>();
            CreateMap<MemberVM, RegisterUserVM>();
            CreateMap<MemberDetailsVM, RegisterUserVM>().ReverseMap();
            CreateMap<Member, MemberProfileVM>().ReverseMap();

            // Trainer mappings
            CreateMap<TrainerVM, Trainer>().ReverseMap();
            CreateMap<TrainerVM, RegisterUserVM>().ReverseMap();
            CreateMap<Trainer, RegisterUserVM>().ReverseMap();

            // Workout mappings
            CreateMap<WorkoutPlan, WorkoutPlanVM>().ReverseMap();
            CreateMap<WorkoutAssignment, WorkoutAssignmentVM>().ReverseMap();
            CreateMap<WorkoutPlanItem, WorkoutPlanItemVM>().ReverseMap();

            // Nutrition mappings
            CreateMap<DietPlan, DietPlanVM>().ReverseMap();
            CreateMap<DietPlanAssignment, DietPlanAssignmentVM>().ReverseMap();
            CreateMap<DietPlanItem, DietPlanItemVM>().ReverseMap();
            CreateMap<MealLog, MealLogVM>().ReverseMap();

            // Other mappings
            CreateMap<Notification, NotificationVM>().ReverseMap();
            CreateMap<Payment, PaymentVM>().ReverseMap();
            CreateMap<Subscription, SubscriptionVM>().ReverseMap();
            CreateMap<Subscription, SubscriptionDetailsVM>().ReverseMap();
            CreateMap<Membership, MembershipVM>().ReverseMap();
            CreateMap<FitnessGoalsVM, FitnessGoals>().ReverseMap();

        }
    }
}
