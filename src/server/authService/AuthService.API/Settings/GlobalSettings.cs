using AuthService.BLL.Abstractions.Settings;
using Common.Auth.Settings;

namespace AuthService.API.Settings;

public class GlobalSettings
{
    public ConnectionStringSettings ConnectionStrings { get; set; } = null!;

    public JwtSettings Jwt { get; set; } = null!;

    /// <summary>AuthService validates its own tokens too, for the [Authorize] /me endpoint.</summary>
    public AuthSettings Auth { get; set; } = null!;

    public CorsSettings Cors { get; set; } = new();

    public CookieSettings Cookies { get; set; } = new();
}
