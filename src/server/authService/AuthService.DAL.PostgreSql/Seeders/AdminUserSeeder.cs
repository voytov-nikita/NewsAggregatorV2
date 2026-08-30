using AuthService.DAL.PostgreSql.Entities;
using Common.Auth.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthService.DAL.PostgreSql.Seeders;

/// <summary>
/// Creates the first admin from configuration. Never ships a default password: if
/// Seed:AdminPassword is not configured the seeder logs a warning and does nothing.
/// Set it with `dotnet user-secrets`, not in a committed file.
/// </summary>
public class AdminUserSeeder : IDataSeeder
{
    public int Order => 3;

    public async Task SeedAsync(IServiceProvider serviceProvider)
    {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        ILogger<AdminUserSeeder> logger = serviceProvider.GetRequiredService<ILogger<AdminUserSeeder>>();

        string? email = configuration["Seed:AdminEmail"];
        string? password = configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Seed:AdminEmail / Seed:AdminPassword are not configured - skipping admin user seeding.");

            return;
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            DisplayName = "Administrator",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };

        IdentityResult result = await userManager.CreateAsync(admin, password);

        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to seed the admin user: {Errors}",
                string.Join("; ", result.Errors.Select(_ => _.Description)));

            return;
        }

        await userManager.AddToRoleAsync(admin, Roles.Admin);

        logger.LogInformation("Seeded admin user {Email}.", email);
    }
}
