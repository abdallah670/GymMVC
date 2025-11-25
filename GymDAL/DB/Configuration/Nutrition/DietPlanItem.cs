using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Nutrition
{
    public class DietPlanItemConfiguration : IEntityTypeConfiguration<DietPlanItem>
    {
        public void Configure(EntityTypeBuilder<DietPlanItem> builder)
        {
            // Table name
            builder.ToTable("DietPlanItems");

            // Primary Key
            builder.HasKey(dpi => dpi.Id);
            builder.Property(dpi => dpi.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(dpi => dpi.DietPlanId)
                .IsRequired();

            builder.Property(dpi => dpi.DayNumber)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(dpi => dpi.MealType)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.Property(dpi => dpi.MealName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dpi => dpi.Calories)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(dpi => dpi.Macros)
                .HasMaxLength(100);

            builder.Property(dpi => dpi.Ingredients)
                .HasMaxLength(500);

            builder.Property(dpi => dpi.Notes)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            // Indexes
            builder.HasIndex(dpi => dpi.DietPlanId);
            builder.HasIndex(dpi => new { dpi.DietPlanId, dpi.DayNumber });
            builder.HasIndex(dpi => dpi.MealType);

            // Composite unique constraint
            builder.HasIndex(dpi => new { dpi.DietPlanId, dpi.DayNumber, dpi.MealType, dpi.MealName })
                .IsUnique();

            // Relationships
            builder.HasOne(dpi => dpi.DietPlan)
                .WithMany(dp => dp.DietPlanItems)
                .HasForeignKey(dpi => dpi.DietPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
