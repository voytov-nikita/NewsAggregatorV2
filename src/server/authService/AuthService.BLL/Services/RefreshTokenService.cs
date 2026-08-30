using AuthService.BLL.Abstractions.Services;
using AuthService.DAL.Abstractions.Stores;
using AuthService.Models.Auth;
using Microsoft.Extensions.Logging;

namespace AuthService.BLL.Services;

public class RefreshTokenService(
    ITokenService tokenService,
    IRefreshTokensStore refreshTokensStore,
    ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    public async Task<RawRefreshTokenModel> IssueAsync(Guid userId, RequestContextModel context)
    {
        RawRefreshTokenModel token = tokenService.CreateRefreshToken();

        await refreshTokensStore.CreateAsync(new CreateRefreshTokenModel
        {
            UserId = userId,
            TokenHash = token.TokenHash,
            ExpiresAt = token.ExpiresAt,
            FamilyId = null,
            CreatedByIp = context.IpAddress,
            UserAgent = context.UserAgent,
        });

        return token;
    }

    public async Task<RefreshRotationResultModel> RotateAsync(string rawToken, RequestContextModel context)
    {
        string hash = tokenService.HashRefreshToken(rawToken);
        RefreshTokenModel? stored = await refreshTokensStore.FindByHashAsync(hash);

        if (stored is null)
        {
            return RefreshRotationResultModel.Failure(RefreshErrorCode.NotFound);
        }

        // Found but already revoked: this token was replayed, so it leaked (stolen, or a buggy
        // client). The safe response is to kill the entire rotation chain, not just this token -
        // whoever holds the newest token in the family is no longer trustworthy either.
        if (stored.IsRevoked)
        {
            await refreshTokensStore.RevokeFamilyAsync(stored.FamilyId, reuseDetected: true);

            logger.LogWarning(
                "Refresh token reuse detected for user {UserId} (family {FamilyId}) from {Ip}. Revoked the whole family.",
                stored.UserId,
                stored.FamilyId,
                context.IpAddress);

            return RefreshRotationResultModel.Failure(RefreshErrorCode.Reused);
        }

        if (stored.IsExpired(DateTime.UtcNow))
        {
            return RefreshRotationResultModel.Failure(RefreshErrorCode.Expired);
        }

        RawRefreshTokenModel replacement = tokenService.CreateRefreshToken();

        RefreshTokenModel created = await refreshTokensStore.CreateAsync(new CreateRefreshTokenModel
        {
            UserId = stored.UserId,
            TokenHash = replacement.TokenHash,
            ExpiresAt = replacement.ExpiresAt,
            FamilyId = stored.FamilyId,
            CreatedByIp = context.IpAddress,
            UserAgent = context.UserAgent,
        });

        await refreshTokensStore.MarkRotatedAsync(stored.Id, created.Id);

        return RefreshRotationResultModel.Success(stored.UserId, replacement);
    }

    public async Task RevokeAsync(string rawToken)
    {
        string hash = tokenService.HashRefreshToken(rawToken);
        RefreshTokenModel? stored = await refreshTokensStore.FindByHashAsync(hash);

        if (stored is not null && !stored.IsRevoked)
        {
            await refreshTokensStore.RevokeAsync(stored.Id);
        }
    }

    public async Task RevokeAllForUserAsync(Guid userId) =>
        await refreshTokensStore.RevokeAllForUserAsync(userId);
}
