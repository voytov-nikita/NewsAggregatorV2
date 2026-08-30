using System.Security.Claims;

using Common.Auth.Constants;

namespace AuthService.API.Extensions.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal) =>
        Guid.TryParse(principal.FindFirst(AuthClaimTypes.Subject)?.Value, out Guid id) ? id : null;
}
