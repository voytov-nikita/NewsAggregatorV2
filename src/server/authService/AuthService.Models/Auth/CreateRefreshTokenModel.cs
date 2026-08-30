namespace AuthService.Models.Auth;

public class CreateRefreshTokenModel
{
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    /// <summary>Null starts a new family; set when rotating an existing token.</summary>
    public Guid? FamilyId { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UserAgent { get; set; }
}
