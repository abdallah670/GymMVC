using System.Collections.Generic;
using GymBLL.ModelVM.Member;

namespace GymBLL.ModelVM.Trainer
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
}
