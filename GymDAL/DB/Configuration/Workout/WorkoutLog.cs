using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Workout
{
    public class WorkoutLogConfiguration : IEntityTypeConfiguration<WorkoutLog>
    {
        public void Configure(EntityTypeBuilder<WorkoutLog> builder)
        {
            // Table name
            builder.ToTable("WorkoutLogs");

            // Primary Key
            builder.HasKey(wl => wl.Id);
            builder.Property(wl => wl.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(wl => wl.WorkoutAssignmentId)
                .IsRequired();

            builder.Property(wl => wl.Date)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(wl => wl.CompletedExercises)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(wl => wl.Notes)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(wl => wl.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(WorkoutStatus.Pending);

            builder.Property(wl => wl.DurationMinutes)
                .HasDefaultValue(0);

            builder.Property(wl => wl.CaloriesBurned)
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(wl => wl.WorkoutAssignmentId);
            builder.HasIndex(wl => wl.Date);
            builder.HasIndex(wl => wl.Status);

            // Composite unique constraint to prevent duplicate logs
            builder.HasIndex(wl => new { wl.WorkoutAssignmentId, wl.Date })
                .IsUnique();

            // Relationships
            builder.HasOne(wl => wl.WorkoutAssignment)
                .WithMany(wa => wa.WorkoutLogs)
                .HasForeignKey(wl => wl.WorkoutAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            //// Computed column for member ID (for easier querying)
            //builder.Property(wl => wl.MemberId)
            //    .HasComputedColumnSql("(SELECT MemberId FROM WorkoutAssignments WHERE Id = WorkoutAssignmentId)");
        }
    }
}
