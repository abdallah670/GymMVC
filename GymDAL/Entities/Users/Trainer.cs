

namespace GymDAL.Entities.Users
{
    public class Trainer : ApplicationUser
    {
       

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(200)]
        public string? Certification { get; set; }
        public decimal HourlyRate { get; set; }
        public int? ExperienceYears { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }
        public int MaxClients { get; set; } = 20; // Maximum number of clients
        public int CurrentClients { get; set; } = 0; // Current number of active clients
        public bool AcceptingNewClients { get; set; } = true;



        public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; }
        public virtual ICollection<DietPlan> DietPlans { get; set; }
        public virtual ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>(); // Memberships they're assigned to
        public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    }
}
