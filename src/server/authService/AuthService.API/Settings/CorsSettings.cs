namespace AuthService.API.Settings;

public class CorsSettings
{
    /// <summary>
    /// Explicit origins are mandatory here: browsers reject AllowAnyOrigin() combined with
    /// credentials, and the refresh cookie needs credentials to travel. The other services keep
    /// their "AllowAll" policy because they only ever see the Authorization header.
    /// </summary>
    public string[] AllowedOrigins { get; set; } = [];
}
