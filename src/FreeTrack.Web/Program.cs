using FreeTrack.Application.Interfaces;
using FreeTrack.Application.Services;
using FreeTrack.Infrastructure.Options;
using FreeTrack.Infrastructure.Persistence;
using FreeTrack.Infrastructure.Persistence.Seed;
using FreeTrack.Infrastructure.Repositories;
using FreeTrack.Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

// Render (and most PaaS hosts) tell the container which port to listen on.
if (Environment.GetEnvironmentVariable("PORT") is { Length: > 0 } port)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// --- Database -----------------------------------------------------------
// EnableRetryOnFailure covers Neon waking up from auto-suspend on the first connection.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        PostgresConnectionString.Normalize(builder.Configuration.GetConnectionString("Default")),
        npgsql => npgsql.EnableRetryOnFailure()));

// --- Repositories & application services --------------------------------
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IStudioServiceRepository, StudioServiceRepository>();
builder.Services.AddScoped<IContactInquiryRepository, ContactInquiryRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAdminNotificationRepository, AdminNotificationRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IStudioServiceQueryService, StudioServiceQueryService>();
builder.Services.AddScoped<IContactInquiryService, ContactInquiryService>();
builder.Services.AddScoped<IBookingService, BookingService>();
// Typed client: 10s timeout so a slow email API can never stall a booking request.
builder.Services.AddHttpClient<IBookingNotificationService, BookingNotificationService>(client =>
{
    client.BaseAddress = new Uri("https://api.resend.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

// Keys live in the database: container disks are ephemeral, and losing them would
// sign everyone out and invalidate open forms on every restart.
builder.Services.AddDataProtection()
    .SetApplicationName("FreeTrack")
    .PersistKeysToDbContext<AppDbContext>();

// Render terminates TLS in front of the container and forwards the original scheme/IP.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// --- Auth -----------------------------------------------------------------
// ASP.NET Core Identity, backed by the same AppDbContext. Same strict policy everywhere:
// local runs use the same remote database as production, so there is no "dev password".
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 10;
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// --- MVC ------------------------------------------------------------------
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Runs in every environment: a fresh Neon database gets its schema and content on first boot.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);

    var adminUsername = app.Configuration["Admin:Username"];
    var adminPassword = app.Configuration["Admin:Password"];
    if (string.IsNullOrEmpty(adminUsername) || string.IsNullOrEmpty(adminPassword))
    {
        app.Logger.LogWarning("Admin:Username / Admin:Password not set — no admin account was seeded.");
    }
    else
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await IdentitySeeder.SeedAdminAsync(userManager, roleManager, adminUsername, adminPassword);
    }
}

app.UseForwardedHeaders();

if (isDevelopment)
{
    app.UseHttpsRedirection();
}
else
{
    // Render already redirects http -> https at its edge.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Deliberately does not touch the database, so uptime checks don't keep Neon awake.
app.MapGet("/healthz", () => Results.Ok("ok"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
