using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Workout
{
    public class WorkoutAssignmentConfiguration : IEntityTypeConfiguration<WorkoutAssignment>
    {
        public void Configure(EntityTypeBuilder<WorkoutAssignment> builder)
        {
            // Table name
            builder.ToTable("WorkoutAssignments");

            // Primary Key
            builder.HasKey(wa => wa.Id);
            builder.Property(wa => wa.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(wa => wa.WorkoutPlanId)
                .IsRequired();

           

            builder.Property(wa => wa.StartDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(wa => wa.EndDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(wa => wa.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(wa => wa.WorkoutPlanId);
           
            builder.HasIndex(wa => wa.StartDate);
            builder.HasIndex(wa => wa.EndDate);
            builder.HasIndex(wa => wa.IsActive);

            // Composite index for active assignments
            builder.HasIndex(wa => new {  wa.IsActive });

            // Relationships
            builder.HasOne(wa => wa.WorkoutPlan)
                .WithMany(wp => wp.WorkoutAssignments)
                .HasForeignKey(wa => wa.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Restrict);

          

        }
    }

}
