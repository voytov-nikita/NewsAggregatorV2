using AuthService.API.Extensions.Discovery;
using AuthService.API.Models.Discovery;
using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Abstractions.Settings;
using Common.Auth.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

/// <summary>
/// Standard discovery endpoints. Consuming services point AddJwtBearer at this service's
/// Authority and fetch the signing keys from here - no key material is ever configured on them.
/// </summary>
[ApiController]
[Route(".well-known")]
public class DiscoveryController : ControllerBase
{
    private readonly IJwksService _jwksService;
    private readonly JwtSettings _jwtSettings;

    public DiscoveryController(IJwksService jwksService, JwtSettings jwtSettings)
    {
        _jwksService = jwksService;
        _jwtSettings = jwtSettings;
    }

    [HttpGet("openid-configuration")]
    public OpenIdConfigurationResponse GetOpenIdConfiguration() => new()
    {
        Issuer = _jwtSettings.Issuer,
        JwksUri = $"{_jwtSettings.Issuer.TrimEnd('/')}/.well-known/jwks.json",
        TokenEndpoint = $"{_jwtSettings.Issuer.TrimEnd('/')}/api/v1/auth/login",
        SigningAlgorithms = ["RS256"],
        GrantTypes = ["password", "refresh_token"],
        SubjectTypes = ["public"],
        Claims =
        [
            AuthClaimTypes.Subject,
            AuthClaimTypes.Email,
            AuthClaimTypes.Name,
            AuthClaimTypes.Role,
            AuthClaimTypes.Permission,
        ],
    };

    [HttpGet("jwks.json")]
    public JwksResponse GetJwks() => new()
    {
        Keys = _jwksService.GetPublicKeys().Select(_ => _.ToResponse()).ToArray(),
    };
}
