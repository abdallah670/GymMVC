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

         

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");


            builder.Property(m => m.CancellationReason)
                .HasMaxLength(500);

            builder.Property(m => m.PreferredTrainingTime)
                .HasMaxLength(50);

            builder.Property(m => m.TrainingIntensity)
                .HasMaxLength(20);

        
         
       
            builder.HasIndex(m => m.MembershipType);
          
            builder.Property(m => m.IsActive)
                .HasDefaultValue(true);

         
        }
    }
}