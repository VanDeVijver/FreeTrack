using FreeTrack.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreeTrack.Infrastructure.Persistence;

// IdentityDbContext adds AspNetUsers/AspNetRoles/etc. alongside the app's own tables.
// IDataProtectionKeyContext stores the cookie/antiforgery keys in the database, so
// logins and open forms survive container restarts and redeploys.
public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<StudioService> StudioServices => Set<StudioService>();
    public DbSet<ContactInquiry> ContactInquiries => Set<ContactInquiry>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<AdminNotification> AdminNotifications => Set<AdminNotification>();
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableUtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // must run first — sets up Identity's own tables
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
