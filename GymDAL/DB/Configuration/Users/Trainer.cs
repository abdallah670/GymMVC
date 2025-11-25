

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configuration.Users
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {
            // Table name
            builder.ToTable("Trainers");

           

        
        }
    }
}
