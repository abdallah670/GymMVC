using GymDAL.Entities.Users;
using GymDAL.Enums;
using GymDAL.Entities.Workout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Financial
{
    public class Subscription
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public int MembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Membership Membership { get; set; }
        public Member Member { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment{ get; set; }
        public virtual DietPlanAssignment? DietPlanAssignment { get; set; }
        public virtual WorkoutAssignment? WorkoutAssignment { get; set; }
        public int? DietPlanAssignmentId { get; set; }
        public int? WorkoutAssignmentId { get; set; }



    }
}
