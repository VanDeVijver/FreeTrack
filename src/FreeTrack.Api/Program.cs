using FreeTrack.Application.Interfaces;
using FreeTrack.Application.Services;
using FreeTrack.Infrastructure.Persistence;
using FreeTrack.Infrastructure.Persistence.Seed;
using FreeTrack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Database -----------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        PostgresConnectionString.Normalize(builder.Configuration.GetConnectionString("Default")),
        npgsql => npgsql.EnableRetryOnFailure()));

// --- Repositories & application services --------------------------------
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IStudioServiceRepository, StudioServiceRepository>();
builder.Services.AddScoped<IContactInquiryRepository, ContactInquiryRepository>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IStudioServiceQueryService, StudioServiceQueryService>();
builder.Services.AddScoped<IContactInquiryService, ContactInquiryService>();

// --- CORS (React/Vite dev server + the deployed front end) --------------
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
