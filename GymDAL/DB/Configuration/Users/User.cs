

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configuration.Users
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Table name
            builder.ToTable("AspNetUsers");

            // Properties
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);


         

            builder.Property(u => u.Phone)
                .HasMaxLength(20);

         

            // Indexes
          
            builder.HasIndex(u => u.FullName);
            builder.HasIndex(u => u.Email);

           
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        
            builder.UseTptMappingStrategy();
        }
    }



}