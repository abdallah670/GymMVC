


using GymDAL.Entities;

namespace TestMVC.DAL.DB
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        // User-related DbSets
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
       
        // Workout-related DbSets
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanItem> WorkoutPlanItems { get; set; }
        public DbSet<WorkoutAssignment> WorkoutAssignments { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }

        // Diet-related DbSets
        public DbSet<DietPlan> DietPlans { get; set; }
        public DbSet<DietPlanItem> DietPlanItems { get; set; }
        public DbSet<DietPlanAssignment> DietPlanAssignments { get; set; }
        public DbSet<MealLog> MealLogs { get; set; }

        // Progress and Notification DbSets
        public DbSet<ProgressLog> ProgressLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ReportLog> ReportLogs { get; set; }

        public DbSet<SystemLog> SystemLogs { get; set; }
        // Financial DbSets
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
            ConfigureGlobalFilters(builder);

            // NO SEED DATA HERE - moved to separate migrations
        }

        private void ConfigureGlobalFilters(ModelBuilder builder)
        {
            // Soft delete filter for all entities that inherit from BaseEntity
            builder.Entity<WorkoutPlan>().HasQueryFilter(wp => wp.IsActive);
            builder.Entity<WorkoutPlanItem>().HasQueryFilter(wpi => wpi.IsActive);
            builder.Entity<WorkoutAssignment>().HasQueryFilter(wa => wa.IsActive);
            builder.Entity<WorkoutLog>().HasQueryFilter(wl => wl.IsActive);
            builder.Entity<DietPlan>().HasQueryFilter(dp => dp.IsActive);
            builder.Entity<DietPlanItem>().HasQueryFilter(dpi => dpi.IsActive);
            builder.Entity<DietPlanAssignment>().HasQueryFilter(dpa => dpa.IsActive);
            builder.Entity<MealLog>().HasQueryFilter(ml => ml.IsActive);
            builder.Entity<ProgressLog>().HasQueryFilter(pl => pl.IsActive);
            builder.Entity<Notification>().HasQueryFilter(n => n.IsActive);
            builder.Entity<ReportLog>().HasQueryFilter(rl => rl.IsActive);
            builder.Entity<ApplicationUser>().HasQueryFilter(m => m.IsActive);
          
        }
    }
}