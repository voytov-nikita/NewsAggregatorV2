namespace AuthService.Models.Users;

public class CreateUserResultModel
{
    public bool Succeeded { get; set; }

    public UserModel? User { get; set; }

    /// <summary>Identity's own validation messages (password rules, duplicate email, ...).</summary>
    public IReadOnlyCollection<string> Errors { get; set; } = [];
}
