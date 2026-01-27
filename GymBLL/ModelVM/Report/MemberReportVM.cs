using GymBLL.ModelVM.Member;
using GymBLL.ModelVM.Workout;
using System;
using System.Collections.Generic;

namespace GymBLL.ModelVM.Report
{
    public class MemberReportVM
    {
        public MemberVM Member { get; set; }
        public string MembershipType { get; set; }
        public DateTime? MembershipEndDate { get; set; }
        
        public int TotalWorkoutsLogged { get; set; }
        public DateTime? LastWorkoutDate { get; set; }
        public double AverageWorkoutDuration { get; set; }
        
        public double StartWeight { get; set; }
        public double CurrentWeight { get; set; }
        public double WeightChange => CurrentWeight - StartWeight;
        
        public List<WorkoutLogVM> RecentLogs { get; set; } = new List<WorkoutLogVM>();
    }
}
