using FreeTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreeTrack.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(120).IsRequired();
        builder.Property(r => r.Slug).HasMaxLength(120).IsRequired();
        builder.HasIndex(r => r.Slug).IsUnique();
        builder.Property(r => r.BadgeLabel).HasMaxLength(60);
        builder.Property(r => r.Subtitle).HasMaxLength(160);
        builder.Property(r => r.Description).HasMaxLength(1000);
        builder.Property(r => r.ImageUrl).HasMaxLength(500);
    }
}
