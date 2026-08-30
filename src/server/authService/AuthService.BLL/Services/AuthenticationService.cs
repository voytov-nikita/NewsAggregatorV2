using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Abstractions.Validators;
using AuthService.DAL.Abstractions.Stores;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using Common.Auth.Constants;

namespace AuthService.BLL.Services;

public class AuthenticationService(
    IUsersStore usersStore,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IAuthValidator validator) : IAuthenticationService
{
    public async Task<AuthResultModel> RegisterAsync(RegisterModel model, RequestContextModel context)
    {
        validator.Validate(model);

        if (await usersStore.FindByEmailAsync(model.Email) is not null)
        {
            return AuthResultModel.Failure(AuthErrorCode.EmailAlreadyTaken, "Email is already registered.");
        }

        CreateUserResultModel created = await usersStore.CreateAsync(
            new CreateUserModel
            {
                Email = model.Email,
                UserName = model.UserName,
                Password = model.Password,
                DisplayName = model.DisplayName,
            },
            Roles.User);

        if (!created.Succeeded || created.User is null)
        {
            return AuthResultModel.Failure(
                AuthErrorCode.ValidationFailed,
                "Registration failed.",
                created.Errors);
        }

        return await IssueTokensAsync(created.User, context);
    }

    public async Task<AuthResultModel> LoginAsync(LoginModel model, RequestContextModel context)
    {
        validator.Validate(model);

        UserModel? user = await usersStore.FindByEmailAsync(model.Email);

        // Same response whether the email is unknown or the password is wrong, so the endpoint
        // cannot be used to enumerate registered addresses.
        if (user is null)
        {
            return InvalidCredentials();
        }

        if (await usersStore.IsLockedOutAsync(user.Id))
        {
            return AuthResultModel.Failure(
                AuthErrorCode.LockedOut,
                "The account is temporarily locked after too many failed attempts.");
        }

        if (!await usersStore.VerifyPasswordAsync(user.Id, model.Password))
        {
            await usersStore.RegisterFailedAttemptAsync(user.Id);

            return InvalidCredentials();
        }

        await usersStore.ResetFailedAttemptsAsync(user.Id);
        await usersStore.SetLastLoginAsync(user.Id, DateTime.UtcNow);

        return await IssueTokensAsync(user, context);
    }

    public async Task<AuthResultModel> RefreshAsync(string refreshToken, RequestContextModel context)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return InvalidRefreshToken();
        }

        RefreshRotationResultModel rotation = await refreshTokenService.RotateAsync(refreshToken, context);

        if (!rotation.Succeeded || rotation.Token is null)
        {
            return InvalidRefreshToken();
        }

        UserModel? user = await usersStore.FindByIdAsync(rotation.UserId);

        // The user may have been deleted or locked out since the token was issued.
        if (user is null || await usersStore.IsLockedOutAsync(user.Id))
        {
            await refreshTokenService.RevokeAllForUserAsync(rotation.UserId);

            return InvalidRefreshToken();
        }

        return AuthResultModel.Success(tokenService.CreateAccessToken(user), rotation.Token, user);
    }

    public async Task LogoutAsync(string? refreshToken, Guid? userId, bool allDevices)
    {
        if (allDevices && userId.HasValue)
        {
            await refreshTokenService.RevokeAllForUserAsync(userId.Value);

            return;
        }

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await refreshTokenService.RevokeAsync(refreshToken);
        }
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId) => await usersStore.FindByIdAsync(userId);

    private async Task<AuthResultModel> IssueTokensAsync(UserModel user, RequestContextModel context)
    {
        AccessTokenModel accessToken = tokenService.CreateAccessToken(user);
        RawRefreshTokenModel refreshToken = await refreshTokenService.IssueAsync(user.Id, context);

        return AuthResultModel.Success(accessToken, refreshToken, user);
    }

    private static AuthResultModel InvalidCredentials() =>
        AuthResultModel.Failure(AuthErrorCode.InvalidCredentials, "Invalid email or password.");

    private static AuthResultModel InvalidRefreshToken() =>
        AuthResultModel.Failure(AuthErrorCode.InvalidRefreshToken, "The refresh token is invalid or expired.");
}
