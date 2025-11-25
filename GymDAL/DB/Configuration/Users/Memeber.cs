

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configuration.Users
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            // Table name
            builder.ToTable("Members");

            // Primary Key
          

            builder.Property(m => m.JoinDate)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("GETUTCDATE()");

       
            builder.HasIndex(m => m.JoinDate);
            builder.HasMany(m => m.Subscriptions)
                .WithOne(s => s.Member)
                .HasForeignKey(s => s.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Payments)
                .WithOne(p => p.Member)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            


        }
    }
}
