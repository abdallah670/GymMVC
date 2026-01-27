using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Nutrition
{
    public class MealLogConfiguration : IEntityTypeConfiguration<MealLog>
    {
        public void Configure(EntityTypeBuilder<MealLog> builder)
        {
            // Table name
            builder.ToTable("MealLogs");

            // Primary Key
            builder.HasKey(ml => ml.Id);
            builder.Property(ml => ml.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(ml => ml.DietPlanAssignmentId)
                .IsRequired();

            builder.Property(ml => ml.Date)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ml => ml.MealsConsumed)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(ml => ml.Notes)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(ml => ml.CaloriesConsumed)
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(ml => ml.DietPlanAssignmentId);
            builder.HasIndex(ml => ml.Date);

            // Composite unique constraint to prevent duplicate logs
       

            // Relationships
            builder.HasOne(ml => ml.DietPlanAssignment)
                .WithMany(dpa => dpa.MealLogs)
                .HasForeignKey(ml => ml.DietPlanAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            //// Computed column for member ID (for easier querying)
            //builder.Property(ml => ml.MemberId)
            //    .HasComputedColumnSql("(SELECT MemberId FROM DietPlanAssignments WHERE Id = DietPlanAssignmentId)");
        }
    }
}
