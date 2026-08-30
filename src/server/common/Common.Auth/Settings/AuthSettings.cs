namespace Common.Auth.Settings;

public class AuthSettings
{
    /// <summary>
    /// AuthService base address. Must match the token's `iss` claim byte for byte,
    /// trailing slash included, or validation fails with IDX10205.
    /// </summary>
    public string Authority { get; set; } = null!;

    public string Audience { get; set; } = null!;

    /// <summary>
    /// Metadata (JWKS) is fetched over HTTPS. Requires a trusted dev certificate locally:
    /// `dotnet dev-certs https --trust`. Set to false only if that is not an option.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;
}
