

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

            builder.Property(m => m.EmergencyContact)
                .HasMaxLength(100);

            builder.Property(m => m.MedicalConditions)
                .HasMaxLength(500);

            // Indexes
       
            builder.HasIndex(m => m.JoinDate);

          

            builder.HasMany(m => m.WorkoutAssignments)
                .WithOne(wa => wa.Member)
                .HasForeignKey(wa => wa.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.DietPlanAssignments)
                .WithOne(dpa => dpa.Member)
                .HasForeignKey(dpa => dpa.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.ProgressLogs)
                .WithOne(pl => pl.Member)
                .HasForeignKey(pl => pl.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.WorkoutLogs)
                .WithOne(wl => wl.Member)
                .HasForeignKey(wl => wl.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.MealLogs)
                .WithOne(ml => ml.Member)
                .HasForeignKey(ml => ml.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
