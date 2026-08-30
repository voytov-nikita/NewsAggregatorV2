using AuthService.DAL.Abstractions.Stores;
using AuthService.DAL.PostgreSql.Entities;
using AuthService.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.PostgreSql.Stores;

public class RefreshTokensStore(AuthServiceDbContext context) : IRefreshTokensStore
{
    public async Task<RefreshTokenModel?> FindByHashAsync(string tokenHash)
    {
        RefreshTokenEntity? entity = await context.RefreshTokens
            .SingleOrDefaultAsync(_ => _.TokenHash == tokenHash);

        return entity is null ? null : ToModel(entity);
    }

    public async Task<RefreshTokenModel> CreateAsync(CreateRefreshTokenModel model)
    {
        var entity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = model.UserId,
            TokenHash = model.TokenHash,
            ExpiresAt = model.ExpiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = model.CreatedByIp,
            UserAgent = model.UserAgent,
        };

        // A token that does not continue a chain starts its own family.
        entity.FamilyId = model.FamilyId ?? entity.Id;

        context.RefreshTokens.Add(entity);
        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task MarkRotatedAsync(Guid tokenId, Guid replacedByTokenId) =>
        await context.RefreshTokens
            .Where(_ => _.Id == tokenId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(_ => _.RevokedAt, DateTime.UtcNow)
                .SetProperty(_ => _.RevokedReason, RefreshTokenRevokedReason.Rotated)
                .SetProperty(_ => _.ReplacedByTokenId, replacedByTokenId));

    public async Task RevokeAsync(Guid tokenId) =>
        await context.RefreshTokens
            .Where(_ => _.Id == tokenId && _.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(_ => _.RevokedAt, DateTime.UtcNow)
                .SetProperty(_ => _.RevokedReason, RefreshTokenRevokedReason.LoggedOut));

    public async Task RevokeFamilyAsync(Guid familyId, bool reuseDetected)
    {
        RefreshTokenRevokedReason reason = reuseDetected
            ? RefreshTokenRevokedReason.ReuseDetected
            : RefreshTokenRevokedReason.AdminRevoked;

        await context.RefreshTokens
            .Where(_ => _.FamilyId == familyId && _.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(_ => _.RevokedAt, DateTime.UtcNow)
                .SetProperty(_ => _.RevokedReason, reason));
    }

    public async Task RevokeAllForUserAsync(Guid userId) =>
        await context.RefreshTokens
            .Where(_ => _.UserId == userId && _.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(_ => _.RevokedAt, DateTime.UtcNow)
                .SetProperty(_ => _.RevokedReason, RefreshTokenRevokedReason.LoggedOut));

    private static RefreshTokenModel ToModel(RefreshTokenEntity entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        FamilyId = entity.FamilyId,
        ExpiresAt = entity.ExpiresAt,
        RevokedAt = entity.RevokedAt,
    };
}
