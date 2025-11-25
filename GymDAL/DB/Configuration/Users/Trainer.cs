

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configuration.Users
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {
            // Table name
            builder.ToTable("Trainers");

           

            builder.Property(t => t.Specialization)
                .HasMaxLength(100);

            builder.Property(t => t.Certification)
                .HasMaxLength(200);

            builder.Property(t => t.Bio)
                .HasMaxLength(1000);

            // Indexes
          
            builder.HasIndex(t => t.Specialization);

      

            builder.HasMany(t => t.WorkoutPlans)
                .WithOne(wp => wp.Trainer)
                .HasForeignKey(wp => wp.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.DietPlans)
                .WithOne(dp => dp.Trainer)
                .HasForeignKey(dp => dp.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
