using GymDAL.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Progress_and_Notification
{
    public class ProgressLogConfiguration : IEntityTypeConfiguration<ProgressLog>
    {
        public void Configure(EntityTypeBuilder<ProgressLog> builder)
        {
            // Table name
            builder.ToTable("ProgressLogs");

            // Primary Key
            builder.HasKey(pl => pl.Id);
            builder.Property(pl => pl.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(pl => pl.MemberId)
                .IsRequired();

            builder.Property(pl => pl.Date)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pl => pl.Weight)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(pl => pl.BodyFat)
                .HasColumnType("decimal(4,2)");

            builder.Property(pl => pl.Chest)
                .HasColumnType("decimal(5,2)");

            builder.Property(pl => pl.Waist)
                .HasColumnType("decimal(5,2)");

            builder.Property(pl => pl.Hips)
                .HasColumnType("decimal(5,2)");

            builder.Property(pl => pl.Arms)
                .HasColumnType("decimal(4,2)");

            builder.Property(pl => pl.WorkoutAdherence)
                .HasDefaultValue(0);

            builder.Property(pl => pl.MealAdherence)
                .HasDefaultValue(0);

            builder.Property(pl => pl.Notes)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(pl => pl.ProgressPhoto)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(pl => pl.MemberId);
            builder.HasIndex(pl => pl.Date);
            builder.HasIndex(pl => pl.Weight);
            builder.HasIndex(pl => pl.BodyFat);

            // Composite unique constraint (one entry per member per day)
            builder.HasIndex(pl => new { pl.MemberId, pl.Date })
                .IsUnique();

            // Relationships
            builder.HasOne(pl => pl.Member)
                .WithMany(m => m.ProgressLogs)
                .HasForeignKey(pl => pl.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }



}
