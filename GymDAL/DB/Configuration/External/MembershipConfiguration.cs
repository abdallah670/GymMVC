using GymDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.ToTable("Memberships");

            // Primary Key
            builder.HasKey(m => m.Id);

            // Properties configuration
            builder.Property(m => m.MembershipType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.PlanName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(m => m.BillingCycle)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(m => m.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            builder.Property(m => m.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Paid");

            builder.Property(m => m.Benefits)
                .HasMaxLength(500);

            builder.Property(m => m.TermsAndConditions)
                .HasMaxLength(1000);

            builder.Property(m => m.CancellationReason)
                .HasMaxLength(500);

            builder.Property(m => m.PreferredTrainingTime)
                .HasMaxLength(50);

            builder.Property(m => m.TrainingIntensity)
                .HasMaxLength(20);

            // Relationships
            builder.HasOne(m => m.Member)
                .WithMany(m => m.Memberships)
                .HasForeignKey(m => m.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Trainer)
                .WithMany(t => t.Memberships)
                .HasForeignKey(m => m.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(m => m.WorkoutPlan)
                .WithMany()
                .HasForeignKey(m => m.WorkoutPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(m => m.DietPlan)
                .WithMany()
                .HasForeignKey(m => m.DietPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(m => m.MemberId);
            builder.HasIndex(m => m.TrainerId);
            builder.HasIndex(m => m.Status);
            builder.HasIndex(m => m.MembershipType);
            builder.HasIndex(m => new { m.StartDate, m.EndDate });

            // Base entity configuration
            builder.Property(m => m.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(m => m.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(m => m.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(m => m.Trainer)
                .WithMany(t => t.Memberships)
                .HasForeignKey(m => m.TrainerId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete membership if trainer is deleted
        }
    }
}