using GymBLL.ModelVM.Identity;


namespace GymBLL.ModelVM.Member
{
    public class MemberProfileVM : UserVM
    {
      

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public bool HasCompletedProfile { get; set; } = false;
        public int Age { get; set; }
        public double CurrentWeight { get; set; }

        public double Height { get; set; }
        public string? Gender { get; set; }
        public int MembershipId { get; set; }
        public string? ActivityLevel { get; set; }
        public string? FitnessGoal { get; set; }
        public int? FitnessGoalId { get; set; }
    }
}
