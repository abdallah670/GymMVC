using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;
using System;
using System.Collections.Generic;

namespace GymPL.ViewModels
{
    public class TrainerDashboardVM
    {
        public int TotalMembers { get; set; }
        public int ActiveWorkoutPlans { get; set; }
        public int ActiveDietPlans { get; set; }
        public List<MemberVM> RecentMembers { get; set; } = new List<MemberVM>();
    }

    public class MemberDashboardVM
    {
        public string MemberName { get; set; }
        public MemberDashboardSubscriptionVM SubscriptionStatus { get; set; }
        public bool HasWorkoutPlan { get; set; }
        public string TodayWorkoutName { get; set; }
        public bool HasDietPlan { get; set; }
        public string TodayDietName { get; set; }
        
        // Items for today
        public List<WorkoutPlanItemVM> TodayExercises { get; set; }
        public List<DietPlanItemVM> TodayMeals { get; set; }
    }

    public class MemberDashboardSubscriptionVM
    {
        public string SubscriptionId { get; set; }
        public string MemberId { get; set; }
        public string MembershipType { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool IsExpired { get; set; }
        public bool CanRenew { get; set; }
        public bool CanUpgrade { get; set; }
        public decimal RenewalPrice { get; set; }
        public string StatusBadgeClass { get; set; }
        public string ProgressPercentage { get; set; }
    }
}
