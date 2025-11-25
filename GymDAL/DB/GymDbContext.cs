


using GymDAL.Entities;
using GymDAL.Entities.External;

namespace TestMVC.DAL.DB
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        // User-related DbSets
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
     
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
       
        // Workout-related DbSets
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanItem> WorkoutPlanItems { get; set; }
        public DbSet<WorkoutAssignment> WorkoutAssignments { get; set; }
       

        // Diet-related DbSets
        public DbSet<DietPlan> DietPlans { get; set; }
        public DbSet<DietPlanItem> DietPlanItems { get; set; }
        public DbSet<DietPlanAssignment> DietPlanAssignments { get; set; }
        public DbSet<MealLog> MealLogs { get; set; }

    
        public DbSet<Notification> Notifications { get; set; }
    
        // Financial DbSets
        public DbSet<Payment> Payments { get; set; }
     
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<FitnessGoals> FitnessGoals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
            ConfigureGlobalFilters(builder);
         
            // NO SEED DATA HERE - moved to separate migrations
        }
        public override int SaveChanges()
        {
            ApplySoftDelete();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplySoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is BaseEntity))
            {
                entry.State = EntityState.Modified;
                var entity = (BaseEntity)entry.Entity;
                entity.IsActive = false;
               
            }
        }
        private void ConfigureGlobalFilters(ModelBuilder builder)
        {
            // Soft delete filter for all entities that inherit from BaseEntity
            builder.Entity<WorkoutPlan>().HasQueryFilter(wp => wp.IsActive);
            builder.Entity<WorkoutPlanItem>().HasQueryFilter(wpi => wpi.IsActive);
            builder.Entity<WorkoutAssignment>().HasQueryFilter(wa => wa.IsActive);

        
            builder.Entity<DietPlan>().HasQueryFilter(dp => dp.IsActive);
            builder.Entity<DietPlanItem>().HasQueryFilter(dpi => dpi.IsActive);
            builder.Entity<DietPlanAssignment>().HasQueryFilter(dpa => dpa.IsActive);
            builder.Entity<MealLog>().HasQueryFilter(ml => ml.IsActive);
          
            builder.Entity<Notification>().HasQueryFilter(n => n.IsActive);
           
            builder.Entity<ApplicationUser>().HasQueryFilter(m => m.IsActive);
            
          
            

        }
    }
}