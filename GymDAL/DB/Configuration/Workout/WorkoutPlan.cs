using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Workout
{

    public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            // Table name
            builder.ToTable("WorkoutPlans");

            // Primary Key
            builder.HasKey(wp => wp.Id);
            builder.Property(wp => wp.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(wp => wp.TrainerId)
                .IsRequired();

            builder.Property(wp => wp.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(wp => wp.Description)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(wp => wp.DurationDays)
                .IsRequired()
                .HasDefaultValue(30);

            builder.Property(wp => wp.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wp => wp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(wp => wp.Difficulty)
                .HasMaxLength(50)
                .HasDefaultValue("Beginner");

            builder.Property(wp => wp.Goal)
                .HasMaxLength(50)
                .HasDefaultValue("General Fitness");
            // Indexes
            builder.HasIndex(wp => wp.TrainerId);
            builder.HasIndex(wp => wp.Name);
            builder.HasIndex(wp => wp.IsActive);
            builder.HasIndex(wp => wp.CreatedAt);

            // Relationships
            builder.HasOne(wp => wp.Trainer)
                .WithMany(t => t.WorkoutPlans)
                .HasForeignKey(wp => wp.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(wp => wp.WorkoutPlanItems)
                .WithOne(wpi => wpi.WorkoutPlan)
                .HasForeignKey(wpi => wpi.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(wp => wp.WorkoutAssignments)
                .WithOne(wa => wa.WorkoutPlan)
                .HasForeignKey(wa => wa.WorkoutPlanId)

                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(wp => wp.Memberships)
              .WithOne(m => m.WorkoutPlan)
              .HasForeignKey(m => m.WorkoutPlanId)
              .OnDelete(DeleteBehavior.Restrict);

            // Base entity configuration
            builder.Property(wp => wp.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wp => wp.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wp => wp.IsActive)
                .HasDefaultValue(true);
        }
    }
}
