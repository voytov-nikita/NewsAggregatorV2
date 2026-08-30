using Microsoft.AspNetCore.Authorization;

namespace Common.Auth.Authorization;

/// <summary>
/// Requires the caller to carry a `permission` claim with the given value.
/// Usage: [HasPermission(Permissions.CommentWrite)].
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = PermissionPolicy.NameFor(permission);
    }
}
