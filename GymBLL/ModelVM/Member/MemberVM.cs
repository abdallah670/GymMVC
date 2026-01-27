using GymBLL.ModelVM.Financial;
using GymBLL.ModelVM.Identity;

namespace GymBLL.ModelVM.Member
{
    public class MemberVM : UserVM
    {
        public DateTime JoinDate { get; set; }
        public FitnessGoalsVM? FitnessGoal { get; set; }
        public double? CurrentWeight { get; set; }
        public double? Height { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string? ActivityLevel { get; set; }
        public bool IsActive { get; set; }
    }
}
