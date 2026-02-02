using System;
using System.Collections.Generic;
using GymBLL.ModelVM.Financial;
using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Workout;
using GymBLL.ModelVM.Nutrition;

namespace GymBLL.ModelVM.Member
{
    public class MemberDashboardVM
    {
        public string MemberName { get; set; }
        public List<WeightLogVM> WeightHistory { get; set; } = new List<WeightLogVM>();
        public int TotalWorkouts { get; set; }
        public MemberDashboardSubscriptionVM SubscriptionStatus { get; set; }
        public bool HasWorkoutPlan { get; set; }
        public bool HasDietPlan { get; set; }
        public string TodayWorkoutName { get; set; }
        public string TodayDietName { get; set; }
        public List<string> ConsistencyLabels { get; set; } = new List<string>();
        public List<int> ConsistencyData { get; set; } = new List<int>();
        
        // Properties from original file
        public string TrainerId { get; set; }
        public string TrainerName { get; set; }
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
        public string StatusBadgeClass { get; set; }
        public bool CanRenew { get; set; } = true;
        public bool CanUpgrade { get; set; } = true;
        public decimal RenewalPrice { get; set; }
        public string ProgressPercentage { get; set; }
    }
}
