namespace AuthService.Models.Users;

public class CreateUserModel
{
    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? DisplayName { get; set; }
}
