namespace AuthService.API.Models.Auth;

/// <summary>
/// Deliberately has no refresh-token property: the refresh token only ever travels in the
/// httpOnly cookie, so a XSS cannot read it. There is no field here for it to leak through.
/// </summary>
public class AuthResponse
{
    public string AccessToken { get; set; } = null!;

    public string TokenType { get; set; } = "Bearer";

    public int ExpiresIn { get; set; }

    public UserResponse User { get; set; } = null!;
}
