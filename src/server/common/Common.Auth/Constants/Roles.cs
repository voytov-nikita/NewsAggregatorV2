namespace Common.Auth.Constants;

public static class Roles
{
    /// <summary>Default role assigned to every user on registration.</summary>
    public const string User = "User";

    public const string Admin = "Admin";

    public static readonly string[] All = [User, Admin];
}
