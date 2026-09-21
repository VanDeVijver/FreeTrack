using FreeTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreeTrack.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.CustomerName).HasMaxLength(120).IsRequired();
        builder.Property(b => b.CustomerEmail).HasMaxLength(200).IsRequired();
        builder.Property(b => b.CustomerPhone).HasMaxLength(30);
        builder.Property(b => b.Message).HasMaxLength(2000);
        builder.Property(b => b.AdminNote).HasMaxLength(1000);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Speeds up the overlap query GetForRoomInRangeAsync runs on every calendar/request check.
        builder.HasIndex(b => new { b.RoomId, b.StartUtc, b.EndUtc });
    }
}
