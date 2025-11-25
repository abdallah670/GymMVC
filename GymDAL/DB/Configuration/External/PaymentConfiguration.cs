using GymDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.PaymentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("USD");

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            builder.Property(p => p.Notes)
                .HasMaxLength(500);

            builder.Property(p => p.BillingName)
                .HasMaxLength(200);

            builder.Property(p => p.BillingAddress)
                .HasMaxLength(500);

            builder.Property(p => p.BillingEmail)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(p => p.MemberId);
            builder.HasIndex(p => p.PaymentDate);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.TransactionId)
                .IsUnique()
                .HasFilter("[TransactionId] IS NOT NULL");

            // Relationships
            builder.HasOne(p => p.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}