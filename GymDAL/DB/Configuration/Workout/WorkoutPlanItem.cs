using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Workout
{
    public class WorkoutPlanItemConfiguration : IEntityTypeConfiguration<WorkoutPlanItem>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlanItem> builder)
        {
            // Table name
            builder.ToTable("WorkoutPlanItems");

            // Primary Key
            builder.HasKey(wpi => wpi.Id);
            builder.Property(wpi => wpi.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(wpi => wpi.WorkoutPlanId)
                .IsRequired();

            builder.Property(wpi => wpi.DayNumber)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(wpi => wpi.ExerciseName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(wpi => wpi.Sets)
                .HasMaxLength(50);

            builder.Property(wpi => wpi.Reps)
                .HasMaxLength(50);

            builder.Property(wpi => wpi.RestDuration)
                .HasMaxLength(50);

            builder.Property(wpi => wpi.Equipment)
                .HasMaxLength(100);

            builder.Property(wpi => wpi.VideoUrl)
                .HasMaxLength(500);

            builder.Property(wpi => wpi.Notes)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            // Indexes
            builder.HasIndex(wpi => wpi.WorkoutPlanId);
            builder.HasIndex(wpi => new { wpi.WorkoutPlanId, wpi.DayNumber });

            // Composite unique constraint
            builder.HasIndex(wpi => new { wpi.WorkoutPlanId, wpi.DayNumber, wpi.ExerciseName })
                .IsUnique();

            // Relationships
            builder.HasOne(wpi => wpi.WorkoutPlan)
                .WithMany(wp => wp.WorkoutPlanItems)
                .HasForeignKey(wpi => wpi.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
