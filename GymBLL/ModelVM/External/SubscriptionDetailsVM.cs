using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.User.Member;
using GymBLL.ModelVM.Workout;
using GymDAL.Entities.Nutrition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBLL.ModelVM.External
{
    public class SubscriptionDetailsVM:SubscriptionVM
    {
        public DietPlanAssignmentVM? DietPlanAssignmentVM { set; get; }
        public WorkoutAssignmentVM? WorkoutAssignmentVM { set; get; }
        public MembershipVM? MembershipVM { set; get; }
        public MemberVM? MemberVM { set; get; }
    }
}
