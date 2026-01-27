using AutoMapper;
using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Trainer;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Communication;
using GymBLL.ModelVM.Financial;
using GymDAL.Entities.Users;
using GymDAL.Entities.Workout;
using GymDAL.Entities.Nutrition;
using GymDAL.Entities.Communication;
using GymDAL.Entities.Financial;
using GymDAL.Entities.Core;
using GymBLL.ModelVM.Identity;
using GymBLL.ModelVM;

namespace GymBLL.Mappper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<UserVM, ApplicationUser>().ReverseMap();
            CreateMap<RegisterUserVM, ApplicationUser>().ReverseMap();

            // Member mappings
          
            CreateMap<TempRegistrationVM, TempRegistration>().ReverseMap();

            CreateMap<Member, MemberVM>().ReverseMap();
            CreateMap<RegisterUserVM, Member>();
            CreateMap<MemberVM, RegisterUserVM>();
           
            CreateMap<Member, MemberProfileVM>().ReverseMap();
            CreateMap<MemberProfileVM, RegisterUserVM>()
                .ForMember(dest => dest.ConfirmPassword, opt => opt.MapFrom(src => src.Password));

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
            CreateMap<ChatMessage, ChatMessageVM>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver.FullName))
                .ForMember(dest => dest.SenderPicture, opt => opt.MapFrom(src => src.Sender.ProfilePicture))
                .ForMember(dest => dest.ReceiverPicture, opt => opt.MapFrom(src => src.Receiver.ProfilePicture))
                .ReverseMap();

        }
    }
}
