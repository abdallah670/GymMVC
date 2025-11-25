using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace GymDAL.Configuration.Users;
public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        // Table name
        builder.ToTable("Admins");


        builder.Property(a => a.IsSuperAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        // Unique constraint - Only one SuperAdmin allowed
        builder.HasIndex(a => a.IsSuperAdmin)
            .HasFilter("[IsSuperAdmin] = 1")
            .IsUnique();

     

       
    }
}