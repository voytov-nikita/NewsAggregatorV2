using AuthService.API.Models.Auth;
using AuthService.Models.Auth;
using AuthService.Models.Users;

namespace AuthService.API.Extensions.Auth;

public static class AuthRequestExtensions
{
    public static RegisterModel ToModel(this RegisterRequest request) => new()
    {
        Email = request.Email,
        UserName = request.UserName,
        Password = request.Password,
        DisplayName = request.DisplayName,
    };

    public static LoginModel ToModel(this LoginRequest request) => new()
    {
        Email = request.Email,
        Password = request.Password,
    };

    public static UserResponse ToResponse(this UserModel model) => new()
    {
        Id = model.Id,
        Email = model.Email,
        UserName = model.UserName,
        DisplayName = model.DisplayName,
        Roles = model.Roles,
        Permissions = model.Permissions,
    };

    public static MeResponse ToMeResponse(this UserModel model) => new()
    {
        Id = model.Id,
        Email = model.Email,
        UserName = model.UserName,
        DisplayName = model.DisplayName,
        Roles = model.Roles,
        Permissions = model.Permissions,
        CreatedAt = model.CreatedAt,
        LastLoginAt = model.LastLoginAt,
    };

    public static AuthResponse ToResponse(this AuthResultModel result) => new()
    {
        AccessToken = result.AccessToken!.Token,
        ExpiresIn = result.AccessToken.ExpiresInSeconds,
        User = result.User!.ToResponse(),
    };
}
