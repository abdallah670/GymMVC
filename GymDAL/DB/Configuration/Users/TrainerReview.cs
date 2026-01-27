

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymDAL.Configuration.Users
{
    public class TrainerReviewConfiguration : IEntityTypeConfiguration<TrainerReview>
    {
        public void Configure(EntityTypeBuilder<TrainerReview> builder)
        {
            // Table name
            builder.ToTable("TrainerReviews");

            // Primary Key


            builder.HasOne(r => r.Trainer).
                WithMany(t=>t.Reviews).
                OnDelete(DeleteBehavior.Restrict);





        }
    }
}
