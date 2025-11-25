

namespace GymDAL.Entities.Users
{
    public class Member : ApplicationUser
    {
       

        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? FitnessGoals { get; set; }
        [StringLength(500)]
        public string? MedicalConditions { get; set; }
        public virtual ICollection<WorkoutAssignment> WorkoutAssignments { get; set; }
        public virtual ICollection<DietPlanAssignment> DietPlanAssignments { get; set; }
        public virtual ICollection<ProgressLog> ProgressLogs { get; set; }
        public virtual ICollection<WorkoutLog> WorkoutLogs { get; set; }
        public virtual ICollection<MealLog> MealLogs { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Membership> Memberships { get; set; }

        
    }
}
