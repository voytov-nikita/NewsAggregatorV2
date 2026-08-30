using System.Security.Cryptography;

using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Abstractions.Settings;
using AuthService.BLL.Security;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using Common.Auth.Constants;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.BLL.Services;

public class TokenService(JwtSettings settings, ISigningKeyProvider signingKeyProvider) : ITokenService
{
    private const int RefreshTokenBytes = 32;

    private readonly JsonWebTokenHandler _handler = new();

    public AccessTokenModel CreateAccessToken(UserModel user)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expiresAt = issuedAt.Add(settings.AccessTokenLifetime);

        var claims = new Dictionary<string, object>
        {
            [AuthClaimTypes.Subject] = user.Id.ToString(),
            [AuthClaimTypes.Email] = user.Email,
            [AuthClaimTypes.Name] = user.DisplayName ?? user.UserName,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),

            // Permissions are flattened from the user's roles at issue time, which means a
            // permission change only takes effect on the next refresh (<= access token lifetime).
            [AuthClaimTypes.Role] = user.Roles.ToArray(),
            [AuthClaimTypes.Permission] = user.Permissions.ToArray(),
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            Claims = claims,
            SigningCredentials = signingKeyProvider.SigningCredentials,
        };

        return new AccessTokenModel
        {
            Token = _handler.CreateToken(descriptor),
            ExpiresAt = expiresAt,
            ExpiresInSeconds = (int)settings.AccessTokenLifetime.TotalSeconds,
        };
    }

    public RawRefreshTokenModel CreateRefreshToken()
    {
        string rawToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(RefreshTokenBytes));

        return new RawRefreshTokenModel
        {
            RawToken = rawToken,
            TokenHash = HashRefreshToken(rawToken),
            ExpiresAt = DateTime.UtcNow.Add(settings.RefreshTokenLifetime),
        };
    }

    /// <summary>
    /// Plain SHA-256, deliberately not PBKDF2/BCrypt. Slow KDFs exist to defeat brute force against
    /// low-entropy secrets; a 256-bit random token is not brute-forceable. A salted slow hash would
    /// also be unindexable, forcing a full table scan on every refresh.
    /// </summary>
    public string HashRefreshToken(string rawToken) =>
        Convert.ToHexStringLower(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
}
