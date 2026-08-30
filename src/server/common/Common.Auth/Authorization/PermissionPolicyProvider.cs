using System.Collections.Concurrent;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Common.Auth.Authorization;

/// <summary>
/// Materializes "perm:&lt;permission&gt;" policies on demand, which is why no policy has to be
/// registered per permission in Program.cs. Anything else falls through to the default provider.
/// </summary>
public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _cache = new();

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!PermissionPolicy.TryGetPermission(policyName, out string permission))
        {
            return base.GetPolicyAsync(policyName);
        }

        AuthorizationPolicy policy = _cache.GetOrAdd(policyName, _ => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission))
            .Build());

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
