using FreeTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreeTrack.Infrastructure.Persistence.Configurations;

public class AdminNotificationConfiguration : IEntityTypeConfiguration<AdminNotification>
{
    public void Configure(EntityTypeBuilder<AdminNotification> builder)
    {
        builder.ToTable("AdminNotifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Message).HasMaxLength(500).IsRequired();

        builder.HasOne(n => n.Booking)
            .WithMany()
            .HasForeignKey(n => n.BookingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
