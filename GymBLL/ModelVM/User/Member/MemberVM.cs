using GymBLL.ModelVM.External;
using GymBLL.ModelVM.User.AppUser;

namespace GymBLL.ModelVM.User.Member
{
    public class MemberVM : UserVM
    {
        public DateTime JoinDate { get; set; }
        public FitnessGoalsVM? FitnessGoal { get; set; }
        public double? CurrentWeight { get; set; }
        public double? CurrentHeight { get; set; }
        public bool? Gender { get; set; }
    }
}
