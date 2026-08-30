using System.Security.Claims;

using Common.Auth.Constants;
using Microsoft.AspNetCore.Http;

namespace Common.Auth.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId =>
        Guid.TryParse(Principal?.FindFirst(AuthClaimTypes.Subject)?.Value, out Guid id) ? id : null;

    public string? Email => Principal?.FindFirst(AuthClaimTypes.Email)?.Value;

    public string? DisplayName => Principal?.FindFirst(AuthClaimTypes.Name)?.Value;

    public IReadOnlyCollection<string> Roles => ValuesOf(AuthClaimTypes.Role);

    public IReadOnlyCollection<string> Permissions => ValuesOf(AuthClaimTypes.Permission);

    public bool HasPermission(string permission) =>
        Principal?.HasClaim(AuthClaimTypes.Permission, permission) == true;

    private IReadOnlyCollection<string> ValuesOf(string claimType) =>
        Principal?.FindAll(claimType).Select(claim => claim.Value).ToArray() ?? [];
}
