using AuthService.Models.Auth;
using AuthService.Models.Users;

namespace AuthService.BLL.Abstractions.Services;

/// <summary>
/// Named AuthenticationService rather than AuthService because the root namespace is already
/// `AuthService` - a type of the same name would shadow it inside this assembly.
/// </summary>
public interface IAuthenticationService
{
    Task<AuthResultModel> RegisterAsync(RegisterModel model, RequestContextModel context);

    Task<AuthResultModel> LoginAsync(LoginModel model, RequestContextModel context);

    Task<AuthResultModel> RefreshAsync(string refreshToken, RequestContextModel context);

    Task LogoutAsync(string? refreshToken, Guid? userId, bool allDevices);

    Task<UserModel?> GetByIdAsync(Guid userId);
}
