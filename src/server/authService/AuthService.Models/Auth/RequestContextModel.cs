namespace AuthService.Models.Auth;

/// <summary>Caller metadata stored alongside a refresh token, for auditing a suspected theft.</summary>
public class RequestContextModel
{
    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}
