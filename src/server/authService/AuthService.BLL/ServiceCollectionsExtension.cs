using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Abstractions.Settings;
using AuthService.BLL.Abstractions.Validators;
using AuthService.BLL.Security;
using AuthService.BLL.Services;
using AuthService.BLL.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.BLL;

public static class ServiceCollectionsExtension
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddSingleton(jwtSettings);

        // Singleton: the RSA key is read from disk once and reused for the process lifetime.
        services.AddSingleton<ISigningKeyProvider, SigningKeyProvider>();

        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IJwksService, JwksService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();

        services.AddTransient<IAuthValidator, AuthValidator>();

        return services;
    }
}
