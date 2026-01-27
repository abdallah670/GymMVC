using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GymPL.ViewModels
{
    public class TrainerDashboardVM
    {
        public int TotalMembers { get; set; }
        public int ActiveWorkoutPlans { get; set; }
        public int ActiveDietPlans { get; set; }
        public List<MemberVM> RecentMembers { get; set; } = new List<MemberVM>();
        
        // Chart Data
        public List<decimal> MonthlyRevenue { get; set; } = new List<decimal>();
        public List<int> NewMembersTrend { get; set; } = new List<int>();
        public List<string> ChartLabels { get; set; } = new List<string>();
    }

    public class MemberDashboardVM
    {
        public string MemberName { get; set; }
        public MemberDashboardSubscriptionVM SubscriptionStatus { get; set; }
        public bool HasWorkoutPlan { get; set; }
        public string TodayWorkoutName { get; set; }
        public bool HasDietPlan { get; set; }
        public string TodayDietName { get; set; }
        public string TrainerId { get; set; }
        public string TrainerName { get; set; }

        // Items for today
        public List<WorkoutPlanItemVM> TodayExercises { get; set; }
        public List<DietPlanItemVM> TodayMeals { get; set; }
        public List<WeightLogVM> WeightHistory { get; set; } = new List<WeightLogVM>();
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

    public class MemberProfileDisplayVM
    {
        public IFormFile ProfilePictureFile { get; set; }
        public string ProfilePicture { get; set; }
        public string MemberId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public double CurrentWeight { get; set; }
        public double Height { get; set; }
        public string FitnessGoal { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public double BMI => Height > 0 ? CurrentWeight / Math.Pow(Height / 100, 2) : 0;
        public MemberDashboardSubscriptionVM SubscriptionStatus { get; set; }
    }
}
