using AuthService.DAL.Abstractions.Stores;
using AuthService.DAL.PostgreSql.Entities;
using AuthService.DAL.PostgreSql.Filters;
using AuthService.DAL.PostgreSql.Seeders;
using AuthService.DAL.PostgreSql.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.DAL.PostgreSql;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AuthServiceDbContext>(builder => builder
            .UseNpgsql(connectionString, optionsBuilder => optionsBuilder.EnableRetryOnFailure())
            .EnableSensitiveDataLogging());

        // Identity options are set explicitly so the rules are readable here rather than
        // implied by framework defaults.
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AuthServiceDbContext>();

        services.AddTransient<IStartupFilter, MigrationsFilter>();

        services.AddTransient<IDataSeeder, PermissionsSeeder>();
        services.AddTransient<IDataSeeder, RolesSeeder>();
        services.AddTransient<IDataSeeder, AdminUserSeeder>();

        services.AddTransient<IUsersStore, UsersStore>();
        services.AddTransient<IPermissionsStore, PermissionsStore>();
        services.AddTransient<IRefreshTokensStore, RefreshTokensStore>();

        return services;
    }
}
