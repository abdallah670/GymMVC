using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Nutrition
{
    public class DietPlanAssignmentConfiguration : IEntityTypeConfiguration<DietPlanAssignment>
    {
        public void Configure(EntityTypeBuilder<DietPlanAssignment> builder)
        {
            // Table name
            builder.ToTable("DietPlanAssignments");

            // Primary Key
            builder.HasKey(dpa => dpa.Id);
            builder.Property(dpa => dpa.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(dpa => dpa.DietPlanId)
                .IsRequired();

            builder.Property(dpa => dpa.MemberId)
                .IsRequired();

            builder.Property(dpa => dpa.StartDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(dpa => dpa.EndDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(dpa => dpa.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(dpa => dpa.DietPlanId);
            builder.HasIndex(dpa => dpa.MemberId);
            builder.HasIndex(dpa => dpa.StartDate);
            builder.HasIndex(dpa => dpa.EndDate);
            builder.HasIndex(dpa => dpa.IsActive);

            // Composite index for active assignments
            builder.HasIndex(dpa => new { dpa.MemberId, dpa.IsActive });

            // Relationships
            builder.HasOne(dpa => dpa.DietPlan)
                .WithMany(dp => dp.DietPlanAssignments)
                .HasForeignKey(dpa => dpa.DietPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dpa => dpa.Member)
                .WithMany(m => m.DietPlanAssignments)
                .HasForeignKey(dpa => dpa.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(dpa => dpa.MealLogs)
                .WithOne(ml => ml.DietPlanAssignment)
                .HasForeignKey(ml => ml.DietPlanAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
