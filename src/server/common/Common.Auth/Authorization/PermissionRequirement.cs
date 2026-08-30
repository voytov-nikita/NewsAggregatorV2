using Microsoft.AspNetCore.Authorization;

namespace Common.Auth.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
