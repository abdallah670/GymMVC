using GymBLL.ModelVM.User.AppUser;
using GymDAL.Entities.External;

namespace GymBLL.ModelVM.User.Member
{
    public class MemberProfileVM:UserVM
    {
        public string Id { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public bool HasCompletedProfile { get; set; } = false;
        public int Age { get; set; }
        public double CurrentWeight { get; set; }
       
        public double Height { get; set; }
        public bool? Gender { get; set; }
        public int MembershipId { get; set; }
       
        public string? FitnessGoal { get; set; }
    }
}