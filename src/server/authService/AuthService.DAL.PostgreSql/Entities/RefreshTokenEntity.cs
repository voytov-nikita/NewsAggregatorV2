namespace AuthService.DAL.PostgreSql.Entities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// SHA-256 of the raw token, not a slow KDF. Slow hashes exist to defeat brute force on
    /// low-entropy secrets; a 256-bit random token is not brute-forceable, and a salted hash
    /// could not be looked up by index — every refresh would scan the table.
    /// </summary>
    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? RevokedAt { get; set; }

    public RefreshTokenRevokedReason? RevokedReason { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>Constant across a rotation chain; equals the Id of the first token in it.</summary>
    public Guid FamilyId { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
