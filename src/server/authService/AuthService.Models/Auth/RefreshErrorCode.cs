namespace AuthService.Models.Auth;

public enum RefreshErrorCode
{
    None = 0,
    NotFound,
    Expired,
    /// <summary>An already-revoked token was presented, which means it leaked. The whole family is revoked.</summary>
    Reused,
}
