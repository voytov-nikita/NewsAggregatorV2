using AuthService.Models.Users;

namespace AuthService.Models.Auth;

public class AuthResultModel
{
    public bool Succeeded { get; set; }

    public AuthErrorCode Error { get; set; }

    public string? Message { get; set; }

    /// <summary>Identity's own messages (password rules, duplicate email, ...).</summary>
    public IReadOnlyCollection<string> ValidationErrors { get; set; } = [];

    public AccessTokenModel? AccessToken { get; set; }

    /// <summary>Never serialized into a response body - it only ever travels in the httpOnly cookie.</summary>
    public RawRefreshTokenModel? RefreshToken { get; set; }

    public UserModel? User { get; set; }

    public static AuthResultModel Success(
        AccessTokenModel accessToken,
        RawRefreshTokenModel refreshToken,
        UserModel user) => new()
    {
        Succeeded = true,
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        User = user,
    };

    public static AuthResultModel Failure(
        AuthErrorCode error,
        string? message = null,
        IReadOnlyCollection<string>? validationErrors = null) => new()
    {
        Succeeded = false,
        Error = error,
        Message = message,
        ValidationErrors = validationErrors ?? [],
    };
}
