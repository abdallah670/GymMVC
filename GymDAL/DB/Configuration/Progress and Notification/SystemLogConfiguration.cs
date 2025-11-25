using GymDAL.Entities.Progress_and_Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configurations
{
    public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.ToTable("SystemLogs");

            builder.HasKey(sl => sl.Id);

            // Properties
            builder.Property(sl => sl.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sl => sl.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sl => sl.EntityType)
                .HasMaxLength(50);

            builder.Property(sl => sl.LogLevel)
                .HasMaxLength(20)
                .HasDefaultValue("Info");

            builder.Property(sl => sl.Notes)
                .HasMaxLength(1000);

            // Indexes
            builder.HasIndex(sl => sl.AdminId);
            builder.HasIndex(sl => sl.Action);
            builder.HasIndex(sl => sl.LogDate);
            builder.HasIndex(sl => sl.LogLevel);
            builder.HasIndex(sl => new { sl.EntityType, sl.EntityId });

            // Relationships
            builder.HasOne(sl => sl.Admin)
                .WithMany(a => a.SystemLogs)
                .HasForeignKey(sl => sl.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}