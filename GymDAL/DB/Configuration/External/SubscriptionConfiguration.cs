using GymDAL.Entities;
using GymDAL.Entities.External;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscription");
            builder.HasKey(s => s.Id);

            // Configure relationships with restricted delete behavior
            builder.HasOne(s => s.Member)
                .WithMany(m => m.Subscriptions)
                .HasForeignKey(s => s.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Membership)
                .WithMany()
                .HasForeignKey(s => s.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Payment)
                .WithMany()
                .HasForeignKey(s => s.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure properties
            builder.Property(s => s.StartDate)
                .IsRequired();

            builder.Property(s => s.EndDate)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();
            builder.Property(s => s.Status)
              .HasConversion<string>();

            builder.HasOne(m => m.WorkoutAssignment)
    .WithMany()
    .HasForeignKey(m => m.WorkoutAssignmentId)
    .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(m => m.DietPlanAssignment)
                .WithMany()
                .HasForeignKey(m => m.DietPlanAssignmentId)
                .OnDelete(DeleteBehavior.SetNull);


        }
    }
}