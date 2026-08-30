namespace AuthService.Models.Auth;

public class RefreshTokenModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid FamilyId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked => RevokedAt is not null;

    public bool IsExpired(DateTime utcNow) => ExpiresAt <= utcNow;
}
