using GymDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configurations
{
    public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
    {
        public void Configure(EntityTypeBuilder<TrainingSession> builder)
        {
            builder.ToTable("TrainingSessions");

            builder.HasKey(ts => ts.Id);

            // Properties
            builder.Property(ts => ts.SessionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ts => ts.FocusArea)
                .HasMaxLength(100);

            builder.Property(ts => ts.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");

            builder.Property(ts => ts.SessionNotes)
                .HasMaxLength(500);

            builder.Property(ts => ts.MemberFeedback)
                .HasMaxLength(500);

            builder.Property(ts => ts.Exercises)
                .HasMaxLength(1000);

            builder.Property(ts => ts.CancellationReason)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(ts => ts.MembershipId);
            builder.HasIndex(ts => ts.TrainerId);
            builder.HasIndex(ts => ts.MemberId);
            builder.HasIndex(ts => ts.SessionDate);
            builder.HasIndex(ts => ts.Status);
            builder.HasIndex(ts => new { ts.TrainerId, ts.SessionDate, ts.StartTime });

            // Relationships
            builder.HasOne(ts => ts.Membership)
                .WithMany(m => m.TrainingSessions)
                .HasForeignKey(ts => ts.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainingSessions)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ts => ts.Member)
                .WithMany()
                .HasForeignKey(ts => ts.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}