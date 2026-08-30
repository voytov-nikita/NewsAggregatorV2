namespace AuthService.API.Models.Auth;

public class MeResponse : UserResponse
{
    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }
}
