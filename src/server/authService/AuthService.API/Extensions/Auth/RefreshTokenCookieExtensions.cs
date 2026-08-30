using AuthService.API.Settings;
using AuthService.Models.Auth;

namespace AuthService.API.Extensions.Auth;

public static class RefreshTokenCookieExtensions
{
    public static void SetRefreshTokenCookie(
        this HttpResponse response,
        RawRefreshTokenModel token,
        CookieSettings settings) =>
        response.Cookies.Append(settings.RefreshTokenName, token.RawToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = settings.SameSiteNone ? SameSiteMode.None : SameSiteMode.Strict,
            Path = settings.Path,
            Expires = token.ExpiresAt,
        });

    public static void ClearRefreshTokenCookie(this HttpResponse response, CookieSettings settings) =>
        response.Cookies.Delete(settings.RefreshTokenName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = settings.SameSiteNone ? SameSiteMode.None : SameSiteMode.Strict,
            Path = settings.Path,
        });

    public static string? GetRefreshToken(this HttpRequest request, CookieSettings settings) =>
        request.Cookies.TryGetValue(settings.RefreshTokenName, out string? token) ? token : null;

    public static RequestContextModel ToRequestContext(this HttpRequest request) => new()
    {
        IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = request.Headers.UserAgent.ToString() is { Length: > 0 } agent ? agent : null,
    };
}
