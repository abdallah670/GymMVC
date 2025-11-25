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

            

            builder.Property(dp => dp.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dp => dp.Description)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(dp => dp.TotalCalories)
                .HasDefaultValue(2000);

          

            builder.Property(dp => dp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
        
            builder.HasIndex(dp => dp.Name);
            builder.HasIndex(dp => dp.IsActive);
            builder.HasIndex(dp => dp.TotalCalories);

       

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

          

            builder.Property(dp => dp.IsActive)
                .HasDefaultValue(true);
        }
    }






}
