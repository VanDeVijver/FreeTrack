using Microsoft.AspNetCore.Identity;

namespace FreeTrack.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the single admin account. Development falls back to the well-known
/// admin/test login below; production must supply Admin:Username/Admin:Password
/// (env vars Admin__Username / Admin__Password) or no admin is created.
/// </summary>
public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public const string DevAdminUsername = "admin";
    public const string DevAdminPassword = "test";

    public static async Task SeedAdminAsync(
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        string username, string password)
    {
        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        var admin = await userManager.FindByNameAsync(username);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = username,
                Email = "admin@freetrack.local",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to seed admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }
}
