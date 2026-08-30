using Common.Auth.Authorization;
using Common.Auth.Constants;
using Common.Auth.Services;
using Common.Auth.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Common.Auth;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Validates access tokens issued by AuthService and enables [HasPermission].
    /// The signing key is fetched from the authority's JWKS endpoint, so no key material is
    /// configured here.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AuthSettings settings)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = settings.Authority;
                options.Audience = settings.Audience;
                options.RequireHttpsMetadata = settings.RequireHttpsMetadata;

                // Keep "sub" / "role" as-is instead of expanding them into WS-Federation schema URIs.
                options.MapInboundClaims = false;

                // Pick up a rotated signing key without restarting this service.
                options.RefreshOnIssuerKeyNotFound = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = AuthClaimTypes.Name,
                    RoleClaimType = AuthClaimTypes.Role,

                    // The default 5 minutes is far too loose for a 15-minute access token.
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
