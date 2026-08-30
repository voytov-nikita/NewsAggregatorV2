namespace AuthService.Models.Auth;

/// <summary>
/// A freshly minted refresh token. <see cref="RawToken"/> is handed to the client exactly once
/// and never stored; only <see cref="TokenHash"/> is persisted.
/// </summary>
public class RawRefreshTokenModel
{
    public string RawToken { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}
