namespace Common.Auth.Authorization;

public static class PermissionPolicy
{
    public const string Prefix = "perm:";

    public static string NameFor(string permission) => Prefix + permission;

    public static bool TryGetPermission(string policyName, out string permission)
    {
        if (policyName.StartsWith(Prefix, StringComparison.Ordinal))
        {
            permission = policyName[Prefix.Length..];

            return permission.Length > 0;
        }

        permission = string.Empty;

        return false;
    }
}
