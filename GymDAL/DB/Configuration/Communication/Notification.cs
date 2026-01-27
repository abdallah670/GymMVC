using GymDAL.Entities.Communication;
using GymDAL.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDAL.Configuration.Communication
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table name
            builder.ToTable("Notifications");

            // Primary Key
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            // Properties
            builder.Property(n => n.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(NotificationStatus.Unread);

            builder.Property(n => n.SendTime)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(n => n.DeliveryMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(DeliveryMethod.InApp);

            builder.Property(n => n.RelatedEntity)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.Type);
            builder.HasIndex(n => n.Status);
            builder.HasIndex(n => n.SendTime);
            builder.HasIndex(n => n.DeliveryMethod);

            // Composite index for efficient notification retrieval
            builder.HasIndex(n => new { n.UserId, n.Status, n.SendTime });

            // Relationships
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}