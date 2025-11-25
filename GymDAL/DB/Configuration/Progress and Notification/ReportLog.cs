using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Entities.Progress_and_Notification
{
    public class ReportLogConfiguration : IEntityTypeConfiguration<ReportLog>
    {
        public void Configure(EntityTypeBuilder<ReportLog> builder)
        {
            // Table name
            builder.ToTable("ReportLogs");

            // Primary Key
            builder.HasKey(rl => rl.Id);
            builder.Property(rl => rl.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(rl => rl.UserId)
                .IsRequired();

            builder.Property(rl => rl.ReportType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(rl => rl.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rl => rl.GeneratedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(rl => rl.ReportName).IsRequired();


            // Indexes
            builder.HasIndex(rl => rl.UserId);
            builder.HasIndex(rl => rl.ReportType);
            builder.HasIndex(rl => rl.GeneratedAt);

            // Relationships
            builder.HasOne(rl => rl.User)
                .WithMany()
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
