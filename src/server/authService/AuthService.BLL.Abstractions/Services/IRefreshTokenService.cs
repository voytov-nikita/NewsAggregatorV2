using AuthService.Models.Auth;

namespace AuthService.BLL.Abstractions.Services;

public interface IRefreshTokenService
{
    /// <summary>Issues a token that starts a new rotation family (login / register).</summary>
    Task<RawRefreshTokenModel> IssueAsync(Guid userId, RequestContextModel context);

    /// <summary>
    /// Validates and rotates a refresh token. Presenting an already-revoked token revokes its
    /// entire family - that is the reuse-detection path.
    /// </summary>
    Task<RefreshRotationResultModel> RotateAsync(string rawToken, RequestContextModel context);

    Task RevokeAsync(string rawToken);

    Task RevokeAllForUserAsync(Guid userId);
}
