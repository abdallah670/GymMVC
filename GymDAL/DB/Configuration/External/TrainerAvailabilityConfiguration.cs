using GymDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configurations
{
    public class TrainerAvailabilityConfiguration : IEntityTypeConfiguration<TrainerAvailability>
    {
        public void Configure(EntityTypeBuilder<TrainerAvailability> builder)
        {
            builder.ToTable("TrainerAvailabilities");

            builder.HasKey(ta => ta.Id);

            // Properties
            builder.Property(ta => ta.DayOfWeek)
                .IsRequired();

            builder.Property(ta => ta.StartTime)
                .IsRequired();

            builder.Property(ta => ta.EndTime)
                .IsRequired();

            builder.Property(ta => ta.AvailabilityType)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Regular");

            builder.Property(ta => ta.Notes)
                .HasMaxLength(200);

            // Complex type configuration
            builder.OwnsOne(ta => ta.SpecificDateRange, dr =>
            {
                dr.Property(d => d.StartDate).HasColumnName("SpecificStartDate");
                dr.Property(d => d.EndDate).HasColumnName("SpecificEndDate");
            });

            // Indexes
            builder.HasIndex(ta => ta.TrainerId);
            builder.HasIndex(ta => ta.DayOfWeek);
            builder.HasIndex(ta => ta.IsAvailable);
            builder.HasIndex(ta => new { ta.TrainerId, ta.DayOfWeek, ta.StartTime, ta.EndTime })
                .IsUnique();

            // Relationships
            builder.HasOne(ta => ta.Trainer)
                .WithMany(t => t.Availabilities)
                .HasForeignKey(ta => ta.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}