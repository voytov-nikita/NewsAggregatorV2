namespace AuthService.Models.Auth;

public class AccessTokenModel
{
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public int ExpiresInSeconds { get; set; }
}
