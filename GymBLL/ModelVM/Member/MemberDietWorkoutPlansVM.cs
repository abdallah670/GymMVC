using GymBLL.ModelVM.Nutrition;
using GymBLL.ModelVM.Workout;
using GymDAL.Entities.Nutrition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBLL.ModelVM.Member
{
    public class MemberDietWorkoutPlansVM : MemberVM
    {
        public DietPlanAssignmentVM? DietPlanAssignmentVM { set; get; }
        public WorkoutAssignmentVM? WorkoutAssignmentVM { set; get; }

    }
}
