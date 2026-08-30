namespace AuthService.DAL.PostgreSql.Entities;

public enum RefreshTokenRevokedReason : short
{
    Rotated = 1,
    LoggedOut = 2,
    ReuseDetected = 3,
    AdminRevoked = 4,
}
