using FreeTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreeTrack.Infrastructure.Persistence.Configurations;

public class ContactInquiryConfiguration : IEntityTypeConfiguration<ContactInquiry>
{
    public void Configure(EntityTypeBuilder<ContactInquiry> builder)
    {
        builder.ToTable("ContactInquiries");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(120).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Phone).HasMaxLength(30);
        builder.Property(c => c.Message).HasMaxLength(2000).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(c => c.Room)
            .WithMany(r => r.Inquiries)
            .HasForeignKey(c => c.RoomId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.StudioService)
            .WithMany(s => s.Inquiries)
            .HasForeignKey(c => c.StudioServiceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
