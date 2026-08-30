namespace Common.Auth.Constants;

/// <summary>
/// Claim types used in access tokens issued by AuthService.
/// Short names on purpose: JwtBearer is configured with MapInboundClaims = false, so no
/// legacy WS-Federation schema URIs are involved.
/// </summary>
public static class AuthClaimTypes
{
    public const string Subject = "sub";

    public const string Email = "email";

    public const string Name = "name";

    public const string Role = "role";

    public const string Permission = "permission";
}
