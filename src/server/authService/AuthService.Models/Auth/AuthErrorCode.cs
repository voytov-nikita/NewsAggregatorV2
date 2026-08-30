namespace AuthService.Models.Auth;

/// <summary>
/// Expected failures, returned as values rather than thrown. The repo's usual style is validators
/// throwing ArgumentException, but UseExceptionHandler turns anything unhandled into a 500 - and a
/// wrong password must be a 401, not a 500.
/// </summary>
public enum AuthErrorCode
{
    None = 0,
    ValidationFailed,
    EmailAlreadyTaken,
    InvalidCredentials,
    LockedOut,
    InvalidRefreshToken,
}
