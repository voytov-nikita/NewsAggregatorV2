using AuthService.Models.Auth;

namespace AuthService.DAL.Abstractions.Stores;

public interface IRefreshTokensStore
{
    Task<RefreshTokenModel?> FindByHashAsync(string tokenHash);

    Task<RefreshTokenModel> CreateAsync(CreateRefreshTokenModel model);

    /// <summary>Marks the old token rotated and links it to its replacement.</summary>
    Task MarkRotatedAsync(Guid tokenId, Guid replacedByTokenId);

    Task RevokeAsync(Guid tokenId);

    /// <summary>
    /// Revokes every live token in a rotation chain. Used when a already-revoked token is
    /// replayed, which means it leaked.
    /// </summary>
    Task RevokeFamilyAsync(Guid familyId, bool reuseDetected);

    Task RevokeAllForUserAsync(Guid userId);
}
