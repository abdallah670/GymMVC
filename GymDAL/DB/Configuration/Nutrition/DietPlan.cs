using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Nutrition
{
    public class DietPlanConfiguration : IEntityTypeConfiguration<DietPlan>
    {
        public void Configure(EntityTypeBuilder<DietPlan> builder)
        {
            // Table name
            builder.ToTable("DietPlans");

            // Primary Key
            builder.HasKey(dp => dp.Id);
            builder.Property(dp => dp.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(dp => dp.TrainerId)
                .IsRequired();

            builder.Property(dp => dp.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dp => dp.Description)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(dp => dp.TotalCalories)
                .HasDefaultValue(2000);

            builder.Property(dp => dp.Macros)
                .HasMaxLength(100);

            builder.Property(dp => dp.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(dp => dp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(dp => dp.TrainerId);
            builder.HasIndex(dp => dp.Name);
            builder.HasIndex(dp => dp.IsActive);
            builder.HasIndex(dp => dp.TotalCalories);

            // Relationships
            builder.HasOne(dp => dp.Trainer)
                .WithMany(t => t.DietPlans)
                .HasForeignKey(dp => dp.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(dp => dp.DietPlanItems)
                .WithOne(dpi => dpi.DietPlan)
                .HasForeignKey(dpi => dpi.DietPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(dp => dp.DietPlanAssignments)
                .WithOne(dpa => dpa.DietPlan)
                .HasForeignKey(dpa => dpa.DietPlanId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(dp => dp.DietType)
              .HasMaxLength(50)
              .HasDefaultValue("Balanced");

            builder.HasMany(dp => dp.Memberships)
                .WithOne(m => m.DietPlan)
                .HasForeignKey(m => m.DietPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Base entity configuration
            builder.Property(dp => dp.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(dp => dp.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(dp => dp.IsActive)
                .HasDefaultValue(true);
        }
    }






}
