using GymDAL.Entities.Core;
using GymDAL.Entities.Financial;
using GymDAL.Entities.Workout;

namespace GymDAL.Entities.Users
{
    public class Member : ApplicationUser
    {
       
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;
        public bool HasCompletedProfile { get; set; } = false;
       
        public int  Age { get; set; }
        public string?  Gender { get; set; }
        public double CurrentWeight { get; set; }
        public string? ActivityLevel { get; set; }
        public double Height { get; set; }
        public int? FitnessGoalId { get; set; }
        public FitnessGoals FitnessGoal { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<Subscription> Subscriptions { get;  set; }
        public List<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
    }
}
