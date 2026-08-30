using Common.Auth.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Common.Auth.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // No shortcut for the Admin role on purpose: Admin is seeded with every permission,
        // so a hardcoded superuser bypass would only hide seeding bugs.
        if (context.User.HasClaim(AuthClaimTypes.Permission, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
