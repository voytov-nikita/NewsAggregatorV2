namespace AuthService.Models.Auth;

public class RegisterModel
{
    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? DisplayName { get; set; }
}
