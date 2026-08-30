using AuthService.Models.Users;

namespace AuthService.DAL.Abstractions.Stores;

/// <summary>
/// Wraps ASP.NET Core Identity so the BLL has no Identity dependency. The wrapper exists
/// mainly for testability: UserManager&lt;T&gt; takes five constructor arguments and has
/// non-virtual members, which makes it painful to mock; this interface does not.
/// </summary>
public interface IUsersStore
{
    Task<CreateUserResultModel> CreateAsync(CreateUserModel model, string role);

    Task<UserModel?> FindByEmailAsync(string email);

    Task<UserModel?> FindByIdAsync(Guid id);

    Task<bool> VerifyPasswordAsync(Guid userId, string password);

    Task<bool> IsLockedOutAsync(Guid userId);

    Task RegisterFailedAttemptAsync(Guid userId);

    Task ResetFailedAttemptsAsync(Guid userId);

    Task SetLastLoginAsync(Guid userId, DateTime loginAt);
}
