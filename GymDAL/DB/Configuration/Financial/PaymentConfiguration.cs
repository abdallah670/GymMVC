using GymDAL.Entities.Financial;
using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.DB.Configuration.Financial
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            // Configure relationship with Member
            builder.HasOne(p => p.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure properties
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.PaymentDate)
                .IsRequired();

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

           
        }
    }
}