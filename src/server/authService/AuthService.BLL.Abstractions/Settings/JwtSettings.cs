namespace AuthService.BLL.Abstractions.Settings;

public class JwtSettings
{
    /// <summary>
    /// Must match the `Authority` configured in every consuming service byte for byte,
    /// trailing slash included - a mismatch surfaces as an opaque IDX10205 at runtime.
    /// </summary>
    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public int AccessTokenMinutes { get; set; } = 15;

    public int RefreshTokenDays { get; set; } = 14;

    /// <summary>
    /// Directory holding the RSA signing key. Resolved to an absolute path in Program.cs so the
    /// key does not land in bin/ and get wiped by a clean.
    /// </summary>
    public string KeysPath { get; set; } = "keys";

    public TimeSpan AccessTokenLifetime => TimeSpan.FromMinutes(AccessTokenMinutes);

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(RefreshTokenDays);
}
