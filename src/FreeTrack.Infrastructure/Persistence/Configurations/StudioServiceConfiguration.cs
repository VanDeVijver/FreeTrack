using FreeTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreeTrack.Infrastructure.Persistence.Configurations;

public class StudioServiceConfiguration : IEntityTypeConfiguration<StudioService>
{
    public void Configure(EntityTypeBuilder<StudioService> builder)
    {
        builder.ToTable("StudioServices");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Number).HasMaxLength(10).IsRequired();
        builder.Property(s => s.Category).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Title).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.ImageUrl).HasMaxLength(500);
    }
}
