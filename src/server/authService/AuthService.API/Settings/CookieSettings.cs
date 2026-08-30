namespace AuthService.API.Settings;

public class CookieSettings
{
    public string RefreshTokenName { get; set; } = "na_rt";

    /// <summary>
    /// Scoped to the auth endpoints, so the cookie is not attached to every request to this service.
    /// </summary>
    public string Path { get; set; } = "/api/v1/auth";

    /// <summary>
    /// The SPA (localhost:4200) and this API (localhost:7330) are different sites, so the cookie
    /// must be SameSite=None - which in turn requires Secure.
    /// </summary>
    public bool SameSiteNone { get; set; } = true;
}
